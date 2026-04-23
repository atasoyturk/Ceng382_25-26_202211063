using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using tastemam.Data;
using tastemam.Models;

namespace tastemam.Controllers
{
    [Authorize(Roles = "Admin,Caretaker")]
    public class CaretakerController : Controller
    {
        private readonly AppDbContext _context;

        public CaretakerController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Caretaker/Index
        public IActionResult Index()
        {
            var menus = _context.MenuItems.ToList();
            return View(menus);
        }

        // GET: /Caretaker/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Caretaker/Create
        [HttpPost]
        public async Task<IActionResult> Create(Menu menu)
        {
            if (!ModelState.IsValid) return View(menu);

            _context.MenuItems.Add(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}