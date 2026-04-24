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

        public IActionResult Index(string kategori = null)
        {
            var menus = _context.MenuItems.AsQueryable();

            if (!string.IsNullOrEmpty(kategori))
                menus = menus.Where(m => m.Category == kategori);
            else
                menus = menus.Take(0);

            ViewData["Kategori"] = kategori;
            return View(menus.ToList());
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