using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> InventorySummary()
        {
            var items = await _context.Inventories.AsNoTracking().ToListAsync();
            return View(items);
        }

        public async Task<IActionResult> SalesReport(DateTime? startDate, DateTime? endDate)
        {
            var today = DateTime.Today;
            var start = (startDate ?? new DateTime(today.Year, today.Month, 1)).Date;
            var end = (endDate ?? today).Date;
            if (start > end)
            {
                return BadRequest("Start date must be on or before the end date.");
            }

            var endExclusive = end.AddDays(1);
            var movements = _context.StockMovements.AsNoTracking()
                .Include(movement => movement.Inventory)
                .Include(movement => movement.Vendor)
                .Where(movement => movement.MovementType == "StockOut" && !movement.IsVoided
                    && movement.CreatedAt >= start && movement.CreatedAt < endExclusive);

            var model = new StockOutReportViewModel
            {
                StartDate = start,
                EndDate = end,
                UnitsSold = await movements.Where(movement => movement.Reason == "Sold")
                    .SumAsync(movement => (int?)movement.Quantity) ?? 0,
                EstimatedSalesValue = await movements.Where(movement => movement.Reason == "Sold")
                    .SumAsync(movement => (decimal?)(movement.Quantity * movement.UnitPrice)) ?? 0m,
                Transactions = await movements.CountAsync(),
                Movements = await movements.OrderByDescending(movement => movement.CreatedAt).ToListAsync()
            };
            return View(model);
        }

        public async Task<IActionResult> TotalInventoryValue()
        {
            var summary = await _context.Inventories.AsNoTracking()
                .GroupBy(item => item.Category)
                .Select(group => new CategorySummaryViewModel
                {
                    Category = group.Key,
                    TotalStock = group.Sum(item => item.Stock),
                    TotalValue = group.Sum(item => item.Stock * item.UnitPrice)
                })
                .ToListAsync();

            ViewBag.GrandTotal = summary.Sum(item => item.TotalValue);
            return View(summary);
        }
    }
}