using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using tastemam.Data;
using tastemam.Models;

namespace tastemam.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly AppDbContext _context;
        private const string CartKey = "Cart";

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString(CartKey);
            return json == null ? new List<CartItem>() : JsonSerializer.Deserialize<List<CartItem>>(json);
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(cart));
        }

        // GET: /Cart/Index
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // POST: /Cart/Add
        [HttpPost]
        public IActionResult Add(int menuId, int quantity, List<int> selectedOptionIds)
        {
            var menu = _context.MenuItems.Find(menuId);
            if (menu == null) return NotFound();

            var selectedOptions = new List<SelectedOption>();
            if (selectedOptionIds != null && selectedOptionIds.Any())
            {
                var options = _context.CustomizationOptions
                    .Where(o => selectedOptionIds.Contains(o.ID))
                    .ToList();

                selectedOptions = options.Select(o => new SelectedOption
                {
                    OptionID = o.ID,
                    OptionName = o.Name,
                    PriceModifier = o.PriceModifier
                }).ToList();
            }

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.MenuID == menuId);

            if (existing != null)
            {
                existing.Quantity += quantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    MenuID = menuId,
                    MenuName = menu.Name,
                    UnitPrice = menu.Price,
                    Quantity = quantity,
                    SelectedOptions = selectedOptions
                });
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // POST: /Cart/UpdateQuantity
        [HttpPost]
        public IActionResult UpdateQuantity(int menuId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.MenuID == menuId);
            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // POST: /Cart/Remove
        [HttpPost]
        public IActionResult Remove(int menuId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.MenuID == menuId);
            SaveCart(cart);
            return RedirectToAction("Index");
        }
    }
}