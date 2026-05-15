using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tastemam.Data;
using X.PagedList.Extensions;

namespace tastemam.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
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