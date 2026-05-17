using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using X.PagedList.Extensions;

using tastemam.Data;


namespace tastemam.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;


        public HomeController(AppDbContext context, IConfiguration config, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
        }

        public IActionResult Index(string kategori = null, double? lat = null, double? lng = null, double radius = 50)
        {
            var menus = _context.MenuItems.AsQueryable();

            if (!string.IsNullOrEmpty(kategori))
                menus = menus.Where(m => m.Category == kategori);
            else
                menus = menus.Take(0);

            // Konum filtresi
            if (lat.HasValue && lng.HasValue)
            {
                var filtered = menus.ToList().Where(m =>
                    m.Latitude.HasValue && m.Longitude.HasValue &&
                    GetDistance(lat.Value, lng.Value, m.Latitude.Value, m.Longitude.Value) <= radius
                ).ToList();

                ViewData["Kategori"] = kategori;
                ViewData["Lat"] = lat;
                ViewData["Lng"] = lng;
                ViewData["Radius"] = radius;
                ViewData["MapsApiKey"] = _config["GoogleMaps:ApiKey"];
                return View(filtered);
            }

            ViewData["Kategori"] = kategori;
            ViewData["Lat"] = lat;
            ViewData["Lng"] = lng;
            ViewData["Radius"] = radius;
            ViewData["MapsApiKey"] = _config["GoogleMaps:ApiKey"];
            return View(menus.ToList());
        }

        public IActionResult Detail(int id)
        {
            var menu = _context.MenuItems
                .Include(m => m.CustomizationGroups)
                .ThenInclude(g => g.Options)
                .FirstOrDefault(m => m.ID == id);

            if (menu == null) return NotFound();

            var menuRatings = _context.Ratings
                .Where(r => r.MenuID == id && r.Type == "Menu")
                .ToList();

            var caretakerRatings = _context.Ratings
                .Where(r => r.CaretakerID == menu.CaretakerID && r.Type == "Caretaker")
                .ToList();

            var caretaker = _userManager.FindByIdAsync(menu.CaretakerID).Result;

            ViewData["MenuAvgRating"] = menuRatings.Any() ? menuRatings.Average(r => r.Score).ToString("0.0") : "—";
            ViewData["MenuRatingCount"] = menuRatings.Count;
            ViewData["CaretakerAvgRating"] = caretakerRatings.Any() ? caretakerRatings.Average(r => r.Score).ToString("0.0") : "—";
            ViewData["CaretakerRatingCount"] = caretakerRatings.Count;
            ViewData["CaretakerEmail"] = caretaker?.Email ?? "—";
            ViewData["MenuComments"] = menuRatings.Where(r => !string.IsNullOrEmpty(r.Comment)).ToList();

            return View(menu);
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "User")]
        public async Task<IActionResult> Orders(string search = null, int page = 1)
        {
            var user = await _userManager.GetUserAsync(User);

            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Menu)
                .Where(o => o.UserID == user.Id)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
                orders = orders.Where(o => o.CustomerName.Contains(search) ||
                                        o.OrderItems.Any(i => i.Menu.Name.Contains(search)));

            orders = orders.OrderByDescending(o => o.Date);

            var pagedOrders = orders.ToPagedList(page, 10);

            ViewData["Search"] = search;
            return View(pagedOrders);
        }

        [Microsoft.AspNetCore.Authorization.Authorize(Roles = "User")]
        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);

            var orders = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Menu)
                .Where(o => o.UserID == user.Id)
                .OrderByDescending(o => o.Date)
                .ToList();

            ViewData["TotalOrders"] = orders.Count;
            ViewData["TotalSpent"] = orders.Sum(o => o.TotalPrice);
            ViewData["CompletedOrders"] = orders.Count(o => o.State == "completed");
            ViewData["RecentOrders"] = orders.Take(5).ToList();

            return View();
        }

        private double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // km
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}