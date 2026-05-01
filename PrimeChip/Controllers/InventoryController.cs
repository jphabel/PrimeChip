using Microsoft.AspNetCore.Mvc;
using PrimeChip.Data;
using PrimeChip.Models;

namespace PrimeChip.Controllers
{
    public class InventoryController : CheckController
    {
        private readonly AppDbContext _context;
        public InventoryController(AppDbContext context) 
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var products = _context.Inventories.ToList();
            return View(products);
        }

        [HttpPost]  
        public IActionResult Create(Inventory Item)
        {
            _context.Inventories.Add(Item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
