using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using tastemam.Data;
using tastemam.Models;
using Microsoft.EntityFrameworkCore;

namespace tastemam.Controllers
{
    [Authorize(Roles = "Admin,Caretaker")]
    public class CaretakerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public CaretakerController(AppDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
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
        public async Task<IActionResult> Create(Menu menu, IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                menu.ImagePath = "/uploads/" + fileName;
            }

            menu.CaretakerID = _userManager.GetUserId(User);

            _context.MenuItems.Add(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: /Caretaker/Edit/5
        public IActionResult Edit(int id)
        {
            var menu = _context.MenuItems.Find(id);
            if (menu == null) return NotFound();
            return View(menu);
        }

        // POST: /Caretaker/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Menu menu, IFormFile imageFile)
        {
            var existing = _context.MenuItems.Find(id);
            if (existing == null) return NotFound();

            existing.Name = menu.Name;
            existing.Description = menu.Description;
            existing.Price = menu.Price;
            existing.Category = menu.Category;

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                existing.ImagePath = "/uploads/" + fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // POST: /Caretaker/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = _context.MenuItems.Find(id);
            if (menu == null) return NotFound();

            _context.MenuItems.Remove(menu);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: /Caretaker/Customize/5
        public IActionResult Customize(int id)
        {
            var menu = _context.MenuItems
                .Include(m => m.CustomizationGroups)
                .ThenInclude(g => g.Options)
                .FirstOrDefault(m => m.ID == id);

            if (menu == null) return NotFound();
            return View(menu);
        }

        // POST: /Caretaker/AddGroup
        [HttpPost]
        public async Task<IActionResult> AddGroup(int menuId, string groupName, string groupType)
        {
            var group = new CustomizationGroup
            {
                MenuID = menuId,
                Name = groupName,
                Type = groupType
            };
            _context.CustomizationGroups.Add(group);
            await _context.SaveChangesAsync();
            return RedirectToAction("Customize", new { id = menuId });
        }

        // POST: /Caretaker/AddOption
        [HttpPost]
        public async Task<IActionResult> AddOption(int groupId, int menuId, string optionName, decimal priceModifier, bool isDefault)
        {
            var option = new CustomizationOption
            {
                CustomizationGroupID = groupId,
                Name = optionName,
                PriceModifier = priceModifier,
                IsDefault = isDefault
            };
            _context.CustomizationOptions.Add(option);
            await _context.SaveChangesAsync();
            return RedirectToAction("Customize", new { id = menuId });
        }

        // POST: /Caretaker/DeleteGroup
        [HttpPost]
        public async Task<IActionResult> DeleteGroup(int groupId, int menuId)
        {
            var group = _context.CustomizationGroups.Find(groupId);
            if (group != null)
            {
                _context.CustomizationGroups.Remove(group);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Customize", new { id = menuId });
        }

        // POST: /Caretaker/DeleteOption
        [HttpPost]
        public async Task<IActionResult> DeleteOption(int optionId, int menuId)
        {
            var option = _context.CustomizationOptions.Find(optionId);
            if (option != null)
            {
                _context.CustomizationOptions.Remove(option);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Customize", new { id = menuId });
        }
    }
}