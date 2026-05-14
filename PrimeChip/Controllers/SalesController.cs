using Microsoft.AspNetCore.Mvc;
using PrimeChip.Data;
using PrimeChip.Migrations;
using PrimeChip.Models;

namespace PrimeChip.Controllers
{
    public class SalesController : CheckController
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var sales = _context.Sales
                .OrderByDescending(s => s.CreatedAt)
                .ToList();
            return View(sales);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StockOut(int inventoryId, int quantity)
        {
            var item = _context.Inventories.Find(inventoryId);
            if (item == null || item.Stock < quantity)
                return RedirectToAction("Index");

            item.Stock -= quantity;

            var sale = new Sale
            {
                InventoryId = item.Id,
                ProductName = item.Name,
                SKU = item.SKU,
                Quantity = quantity,
                UnitPrice = item.UnitPrice,
                TotalAmount = quantity * item.UnitPrice,
                Type = "Stock Out",
                CreatedAt = DateTime.Now
            };

            _context.Sales.Add(sale);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}