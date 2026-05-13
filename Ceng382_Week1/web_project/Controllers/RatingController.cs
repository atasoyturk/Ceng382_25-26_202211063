using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tastemam.Data;
using tastemam.Models;
using tastemam.Services;

namespace tastemam.Controllers
{
    [Authorize(Roles = "User")]
    public class RatingController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly LogService _logService;

        public RatingController(AppDbContext context, UserManager<IdentityUser> userManager, LogService logService)
        {
            _context = context;
            _userManager = userManager;
            _logService = logService;
        }

        // GET: /Rating/Create?orderId=5
        public async Task<IActionResult> Create(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Menu)
                .FirstOrDefault(o => o.ID == orderId);

            if (order == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (order.UserID != user.Id) return Forbid();

            ViewData["OrderId"] = orderId;
            return View(order);
        }

        // POST: /Rating/SubmitMenuRating
        [HttpPost]
        public async Task<IActionResult> SubmitMenuRating(int orderId, int menuId, int score, string comment)
        {
            var user = await _userManager.GetUserAsync(User);

            var existing = _context.Ratings.FirstOrDefault(r =>
                r.OrderID == orderId && r.MenuID == menuId && r.UserID == user.Id);

            if (existing != null)
            {
                existing.Score = score;
                existing.Comment = comment;
                existing.Date = DateTime.Now;
            }
            else
            {
                _context.Ratings.Add(new Rating
                {
                    MenuID = menuId,
                    UserID = user.Id,
                    OrderID = orderId,
                    Score = score,
                    Comment = comment,
                    Date = DateTime.Now,
                    Type = "Menu"
                });
            }

            await _context.SaveChangesAsync();
            await _logService.LogAsync("Rating", $"Menu ID: {menuId}, Puan: {score}", user.Email);

            return RedirectToAction("Create", new { orderId });
        }

        // POST: /Rating/SubmitCaretakerRating
        [HttpPost]
        public async Task<IActionResult> SubmitCaretakerRating(int orderId, string caretakerId, int score, string comment)
        {
            var user = await _userManager.GetUserAsync(User);

            var existing = _context.Ratings.FirstOrDefault(r =>
                r.OrderID == orderId && r.CaretakerID == caretakerId && r.UserID == user.Id && r.Type == "Caretaker");

            if (existing != null)
            {
                existing.Score = score;
                existing.Comment = comment;
                existing.Date = DateTime.Now;
            }
            else
            {
                _context.Ratings.Add(new Rating
                {
                    CaretakerID = caretakerId,
                    UserID = user.Id,
                    OrderID = orderId,
                    Score = score,
                    Comment = comment,
                    Date = DateTime.Now,
                    Type = "Caretaker"
                });
            }

            await _context.SaveChangesAsync();
            await _logService.LogAsync("Rating", $"Caretaker ID: {caretakerId}, Puan: {score}", user.Email);

            return RedirectToAction("Create", new { orderId });
        }
    }
}