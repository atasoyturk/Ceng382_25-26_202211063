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
        var all = _context.ShipperContactInfos.ToList();
        var active = all.Where(x => x.IsActive).ToList();
        ViewBag.ActiveList = active;
        return View(all);
                
    }

    // GET
    public IActionResult Edit(int id)
    {
        var item = _context.ShipperContactInfos.Find(id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ShipperContactInfo updated)
    {
        var item = _context.ShipperContactInfos.Find(id);
        if (item == null) return NotFound();

        item.Email = updated.Email;
        item.Website = updated.Website;
        item.Phone = updated.Phone;
        item.Address = updated.Address;
        item.City = updated.City;
        item.Country = updated.Country;
        item.PostalCode = updated.PostalCode;
        item.IsActive = updated.IsActive;

        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var item = _context.ShipperContactInfos.Find(id);
        if (item == null) return NotFound();

        _context.ShipperContactInfos.Remove(item);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

     // POST Hard Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult HardDelete()
    {
        var activeItems = _context.ShipperContactInfos.Where(x => x.IsActive).ToList();
        _context.ShipperContactInfos.RemoveRange(activeItems);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
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
