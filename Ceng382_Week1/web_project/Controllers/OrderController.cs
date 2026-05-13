using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private const string CartKey = "Cart";

        public OrderController(AppDbContext context, UserManager<IdentityUser> userManager, LogService logService)
        {
            _context = context;
            _userManager = userManager;
            _logService = logService;
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

            await _logService.LogAsync("Order", $"Sipariş oluşturuldu. Sipariş ID: {order.ID}, Toplam: {order.TotalPrice}₺", user.Email);
            await _logService.LogAsync("Payment", $"Ödeme tamamlandı. Sipariş ID: {order.ID}, Tutar: {order.TotalPrice}₺", user.Email);

            HttpContext.Session.Remove(CartKey);

            return RedirectToAction("Confirmation", new { id = order.ID });
        }

        public IActionResult Confirmation(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}