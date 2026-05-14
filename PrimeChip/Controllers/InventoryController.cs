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
            Item.InitialStock = Item.Stock;
            _context.Inventories.Add(Item);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Inventories.FindAsync(id);
            if (item != null)
            {
                _context.Inventories.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}