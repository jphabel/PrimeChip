using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeChip.Data;
using PrimeChip.Models;

namespace PrimeChip.Controllers
{
    public class AlertsController : CheckController
    {
        private readonly AppDbContext _context;
        public AlertsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var alerts = _context.Inventories
                .Where(i => i.Stock <= i.ReorderLevel)
                .ToList();

            return View(alerts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Purchase(int inventoryId, int quantity)
        {
            var item = _context.Inventories.Find(inventoryId);
            if (item == null) return NotFound();


            item.Stock += quantity;

            var sale = new Sale
            {
                InventoryId = item.Id,
                ProductName = item.Name,
                SKU = item.SKU,
                Quantity = quantity,
                UnitPrice = item.UnitPrice,
                TotalAmount = quantity * item.UnitPrice,
                Type = "Stock In",
                CreatedAt = DateTime.Now
            };

            _context.Sales.Add(sale);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
