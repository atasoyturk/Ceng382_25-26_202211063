using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tastemam.Data;
using tastemam.Services;
using X.PagedList.Extensions;

namespace tastemam.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly LogService _logService;

        public AdminController(AppDbContext context, UserManager<IdentityUser> userManager, LogService logService)
        {
            _context = context;
            _userManager = userManager;
            _logService = logService;
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

            await _logService.LogAsync("Admin", "Admin dashboard görüntülendi.", User.Identity.Name);
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userManager.GetUsersInRoleAsync("User");
            await _logService.LogAsync("Admin", "Kullanıcı listesi görüntülendi.", User.Identity.Name);
            return View(users);
        }

        public async Task<IActionResult> Caretakers()
        {
            var caretakers = await _userManager.GetUsersInRoleAsync("Caretaker");

            var agreements = _context.CaretakerAgreements
                .ToDictionary(a => a.CaretakerID, a => a.IsApproved);

            var menuCounts = _context.MenuItems
                .GroupBy(m => m.CaretakerID)
                .ToDictionary(g => g.Key, g => g.Count());

            ViewData["Agreements"] = agreements;
            ViewData["MenuCounts"] = menuCounts;

            await _logService.LogAsync("Admin", "Caretaker listesi görüntülendi.", User.Identity.Name);
            return View(caretakers);
        }

        public async Task<IActionResult> Orders(int page = 1)
        {
            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Menu)
                .OrderByDescending(o => o.Date)
                .ToPagedList(page, 20);

            await _logService.LogAsync("Admin", "Sipariş listesi görüntülendi.", User.Identity.Name);
            return View(orders);
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