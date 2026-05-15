using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using tastemam.Data;
using tastemam.Models;
using tastemam.Services;

namespace tastemam.Controllers
{
    [Authorize(Roles = "User")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly LogService _logService;
        private readonly EmailService _emailService;
        private readonly PdfService _pdfService;

        private const string CartKey = "Cart";

        public OrderController(AppDbContext context, UserManager<IdentityUser> userManager, LogService logService, EmailService emailService, PdfService pdfService)
        {
            _context = context;
            _userManager = userManager;
            _logService = logService;
            _emailService = emailService;
            _pdfService = pdfService;
        }

        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString(CartKey);
            return json == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(json);
        }

        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");
            return View(cart);
        }

        public IActionResult Payment()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOrder(string cardNumber, string cardHolder, string expiryDate, string cvv)
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");

            var user = await _userManager.GetUserAsync(User);

            var order = new Order
            {
                CustomerName = user.Email,
                Phone = "",
                Date = DateTime.Now,
                TotalPrice = cart.Sum(i => i.TotalPrice),
                State = "pending",
                UserID = user.Id,
                OrderItems = new List<OrderItem>()
            };

            foreach (var cartItem in cart)
            {
                var orderItem = new OrderItem
                {
                    MenuID = cartItem.MenuID,
                    Pieces = cartItem.Quantity,
                    UnitPrice = cartItem.UnitPrice,
                    SelectedCustomizations = cartItem.SelectedOptions.Select(o => new OrderItemCustomization
                    {
                        CustomizationOptionID = o.OptionID
                    }).ToList()
                };
                order.OrderItems.Add(orderItem);
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _logService.LogAsync("Order", $"Sipariş oluşturuldu. ID: {order.ID}, Toplam: {order.TotalPrice}₺", user.Email);
            await _logService.LogAsync("Payment", $"Ödeme tamamlandı. Sipariş ID: {order.ID}, Tutar: {order.TotalPrice}₺", user.Email);

            // Email gönder
            var itemsSummary = string.Join("\n", cart.Select(i => $"{i.MenuName} x{i.Quantity} = {i.TotalPrice}₺"));
            try
            {
                await _emailService.SendOrderConfirmationAsync(user.Email, order.ID, order.TotalPrice, itemsSummary);

                // Caretaker'a bildirim gönder
                var caretakerIds = cart.Select(i => i.MenuID).Distinct().ToList();
                var menus = _context.MenuItems.Where(m => caretakerIds.Contains(m.ID)).ToList();
                var caretakerEmails = new HashSet<string>();

                foreach (var menu in menus)
                {
                    if (!string.IsNullOrEmpty(menu.CaretakerID))
                    {
                        var caretaker = await _userManager.FindByIdAsync(menu.CaretakerID);
                        if (caretaker != null && !caretakerEmails.Contains(caretaker.Email))
                        {
                            caretakerEmails.Add(caretaker.Email);
                            await _emailService.SendOrderNotificationToCaretakerAsync(caretaker.Email, order.ID, user.Email, order.TotalPrice);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await _logService.LogAsync("Error", $"Email gönderilemedi: {ex.Message}", user.Email, "Error");
            }

            HttpContext.Session.Remove(CartKey);
            return RedirectToAction("Confirmation", new { id = order.ID });
        }

        public IActionResult Confirmation(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();
            return View(order);
        }

        public IActionResult DownloadReceipt(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Menu)
                .FirstOrDefault(o => o.ID == id);

            if (order == null) return NotFound();

            var pdf = _pdfService.GenerateOrderReceipt(order, order.OrderItems.ToList());
            return File(pdf, "application/pdf", $"tastemam-fis-{id}.pdf");
        }

    }
}