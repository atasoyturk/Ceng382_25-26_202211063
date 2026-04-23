using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tastemam.Data;

namespace tastemam.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var menus = _context.MenuItems.ToList();
            return View(menus);
        }

        public IActionResult Detail(int id)
        {
            var menu = _context.MenuItems
                .Include(m => m.CustomizationGroups)
                .ThenInclude(g => g.Options)
                .FirstOrDefault(m => m.ID == id);

            if (menu == null) return NotFound();
            return View(menu);
        }
    }
}