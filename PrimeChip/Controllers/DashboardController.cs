using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeChip.Data;
using PrimeChip.Models;

namespace PrimeChip.Controllers
{
    public class DashboardController : CheckController
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var products = _context.Inventories.AsNoTracking();
            var soldToday = _context.StockMovements.AsNoTracking()
                .Where(movement => movement.MovementType == "StockOut"
                    && movement.Reason == "Sold"
                    && !movement.IsVoided
                    && movement.CreatedAt >= today
                    && movement.CreatedAt < tomorrow);

            var model = new DashboardViewModel
            {
                TotalProducts = await products.CountAsync(),
                TotalStock = await products.SumAsync(item => (int?)item.Stock) ?? 0,
                LowStock = await products.CountAsync(item => item.Stock > 0 && item.Stock <= item.ReorderLevel),
                OutOfStock = await products.CountAsync(item => item.Stock == 0),
                TotalInventoryValue = await products.SumAsync(item => (decimal?)(item.Stock * item.UnitPrice)) ?? 0m,
                UnitsSoldToday = await soldToday.SumAsync(movement => (int?)movement.Quantity) ?? 0,
                SalesValueToday = await soldToday.SumAsync(movement => (decimal?)(movement.Quantity * movement.UnitPrice)) ?? 0m,
                StockOutsToday = await _context.StockMovements.CountAsync(movement =>
                    movement.MovementType == "StockOut" && !movement.IsVoided
                    && movement.CreatedAt >= today && movement.CreatedAt < tomorrow),
                Alerts = await products.Where(item => item.Stock <= item.ReorderLevel)
                    .OrderBy(item => item.Stock).ThenBy(item => item.Name).Take(8).ToListAsync(),
                RecentMovements = await _context.StockMovements.AsNoTracking()
                    .Include(movement => movement.Inventory)
                    .Include(movement => movement.Vendor)
                    .OrderByDescending(movement => movement.CreatedAt).Take(8).ToListAsync(),
                TopStockItems = await products.OrderByDescending(item => item.Stock).Take(5).ToListAsync()
            };

            var months = Enumerable.Range(0, 7).Select(offset => new DateTime(today.Year, today.Month, 1).AddMonths(offset - 6)).ToList();
            model.ChartLabels = months.Select(month => month.ToString("MMM yyyy")).ToList();
            foreach (var month in months)
            {
                var nextMonth = month.AddMonths(1);
                var movements = _context.StockMovements.AsNoTracking().Where(movement =>
                    movement.MovementType == "StockOut" && movement.Reason == "Sold" && !movement.IsVoided
                    && movement.CreatedAt >= month && movement.CreatedAt < nextMonth);
                model.ChartQty.Add(await movements.SumAsync(movement => (int?)movement.Quantity) ?? 0);
                model.ChartValue.Add(await movements.SumAsync(movement => (decimal?)(movement.Quantity * movement.UnitPrice)) ?? 0m);
            }

            return View(model);
        }
    }
}
