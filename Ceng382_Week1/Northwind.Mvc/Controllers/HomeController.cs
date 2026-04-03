using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller
{

    private readonly NorthwindContext _context;

    public HomeController(NorthwindContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var shipperContactInfo = _context.ShipperContactInfos.ToList();
        return View(shipperContactInfo);
                
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
