using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tastemam.Data;

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

        public IActionResult Logs(string eventType = null, string level = null)
        {
            var logs = _context.SystemLogs.AsQueryable();

            if (!string.IsNullOrEmpty(eventType))
                logs = logs.Where(l => l.EventType == eventType);

            if (!string.IsNullOrEmpty(level))
                logs = logs.Where(l => l.Level == level);

            logs = logs.OrderByDescending(l => l.Date);

            ViewData["EventType"] = eventType;
            ViewData["Level"] = level;

            return View(logs.ToList());
        }
    }
}