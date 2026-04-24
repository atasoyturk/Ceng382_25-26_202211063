using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using tastemam.Data;
using tastemam.Models;

namespace tastemam.Controllers
{
    [Authorize(Roles = "User")]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private const string CartKey = "Cart";

        public OrderController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString(CartKey);
            return json == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(json);
        }

        // GET: /Order/Checkout
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");
            return View(cart);
        }

        // GET: /Order/Payment
        public IActionResult Payment()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");
            return View(cart);
        }

        // POST: /Order/CompleteOrder
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

            HttpContext.Session.Remove(CartKey);

            return RedirectToAction("Confirmation", new { id = order.ID });
        }

        // GET: /Order/Confirmation
        public IActionResult Confirmation(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null) return NotFound();
            return View(order);
        }
    }
}