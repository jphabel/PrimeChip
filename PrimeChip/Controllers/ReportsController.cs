using Microsoft.AspNetCore.Mvc;
using PrimeChip.Data;
using PrimeChip.Models;

namespace PrimeChip.Controllers
{
    public class ReportsController : CheckController
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        public IActionResult InventorySummary()
        {
            var items = _context.Inventories.ToList();
            return View(items);
        }

        public IActionResult SalesReport()
        {
            var sales = _context.Sales.OrderByDescending(s => s.CreatedAt).ToList();
            return View(sales);
        }

        public IActionResult TotalInventoryValue()
        {
            var summary = _context.Inventories
                .GroupBy(i => i.Category)
                .Select(g => new CategorySummaryViewModel
                {
                    Category = g.Key,
                    TotalStock = g.Sum(i => i.Stock),
                    TotalValue = g.Sum(i => i.Stock * i.UnitPrice)
                })
                .ToList();

            ViewBag.GrandTotal = summary.Sum(s => s.TotalValue);
            return View(summary);
        }
    }
}