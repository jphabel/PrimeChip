using Microsoft.AspNetCore.Mvc;
using PrimeChip.Data;


namespace PrimeChip.Controllers
{
    public class DashboardController : CheckController
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var totalValue = _context.Inventories
                .Sum(i => i.Stock * i.UnitPrice);

            ViewBag.TotalInventoryValue = Convert.ToDecimal(totalValue);


            var totalInventory = _context.Inventories
                .Sum(i => i.Stock);

            ViewBag.TotalStock = totalInventory;

            var lowStock = _context.Inventories
                .Count(i => i.Stock <= i.ReorderLevel);

            ViewBag.LowStock = lowStock;


            var outOfStock = _context.Inventories
                .Count(i => i.Stock == 0);

            ViewBag.OutOfStock = outOfStock;

            var now = DateTime.Now;
            var months = Enumerable.Range(0, 7)
                .Select(i => now.AddMonths(-6 + i))
                .ToList();

            var monthlyLabels = months.Select(m => m.ToString("MMMM")).ToList();

            var monthlyQty = months.Select(m =>
                _context.Inventories
                    .Where(i => i.CreatedAt.Month == m.Month && i.CreatedAt.Year == m.Year)
                    .Sum(i => (int?)i.Stock) ?? 0
            ).ToList();

            var monthlyValue = months.Select(m =>
                _context.Inventories
                    .Where(i => i.CreatedAt.Month == m.Month && i.CreatedAt.Year == m.Year)
                    .Sum(i => (decimal?)(i.Stock * i.UnitPrice)) ?? 0
            ).ToList();

            ViewBag.ChartLabels = System.Text.Json.JsonSerializer.Serialize(monthlyLabels);
            ViewBag.ChartQty = System.Text.Json.JsonSerializer.Serialize(monthlyQty);
            ViewBag.ChartValue = System.Text.Json.JsonSerializer.Serialize(monthlyValue);
            ViewBag.TotalProducts = _context.Inventories.Count();
            ViewBag.LowStock = _context.Inventories.Count(i => i.Stock > 0 && i.Stock < 10);
            ViewBag.OutOfStock = _context.Inventories.Count(i => i.Stock == 0);
            ViewBag.TotalValue = _context.Inventories.Sum(i => (decimal?)(i.Stock * i.UnitPrice)) ?? 0;

            return View();
        }
    }
}
