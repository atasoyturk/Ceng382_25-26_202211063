using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tastemam.Data;
using Microsoft.AspNetCore.Identity;
using X.PagedList.Extensions;

namespace tastemam.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public AdminController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var totalUsers = await _userManager.GetUsersInRoleAsync("User");
            var totalCaretakers = await _userManager.GetUsersInRoleAsync("Caretaker");

            ViewData["TotalUsers"] = totalUsers.Count;
            ViewData["TotalCaretakers"] = totalCaretakers.Count;
            ViewData["TotalOrders"] = _context.Orders.Count();
            ViewData["TotalMenuItems"] = _context.MenuItems.Count();
            ViewData["TotalLogs"] = _context.SystemLogs.Count();
            ViewData["RecentLogs"] = _context.SystemLogs
                .OrderByDescending(l => l.Date)
                .Take(5)
                .ToList();

            return View();
        }

        public IActionResult Logs(string eventType = null, string level = null, string search = null, int page = 1)
        {
            var logs = _context.SystemLogs.AsQueryable();

            if (!string.IsNullOrEmpty(eventType))
                logs = logs.Where(l => l.EventType == eventType);

            if (!string.IsNullOrEmpty(level))
                logs = logs.Where(l => l.Level == level);

            if (!string.IsNullOrEmpty(search))
                logs = logs.Where(l => l.Message.Contains(search) || l.UserEmail.Contains(search));

            logs = logs.OrderByDescending(l => l.Date);

            var pagedLogs = logs.ToPagedList(page, 20);

            ViewData["EventType"] = eventType;
            ViewData["Level"] = level;
            ViewData["Search"] = search;

            return View(pagedLogs);
        }
    }
}