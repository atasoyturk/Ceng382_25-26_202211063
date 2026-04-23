using Microsoft.AspNetCore.Mvc;
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
    }
}