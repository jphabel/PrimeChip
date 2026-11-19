using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeChip.Data;
using PrimeChip.Models;

namespace PrimeChip.Controllers
{
    public class StockOutController : CheckController
    {
        private static readonly string[] AllowedReasons = ["Sold", "Damaged/Defective", "Internal Use"];
        private readonly AppDbContext _context;

        public StockOutController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date)
        {
            var filterDate = (date ?? DateTime.Today).Date;
            var model = new StockOutViewModel { Date = DateTime.Today, FilterDate = filterDate };
            await PopulateAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockOutViewModel model)
        {
            model.ProductName = model.ProductName?.Trim() ?? string.Empty;
            model.Category = model.Category?.Trim() ?? string.Empty;
            var allowedCategories = await GetCategoriesAsync();
            if (!allowedCategories.Contains(model.Category, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.Category), "Select a valid product category.");
            }

            if (!AllowedReasons.Contains(model.Reason, StringComparer.Ordinal))
            {
                ModelState.AddModelError(nameof(model.Reason), "Select a valid stock-out reason.");
            }

            if (!ModelState.IsValid)
            {
                model.FilterDate = DateTime.Today;
                await PopulateAsync(model);
                return View("Index", model);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            var product = await _context.Inventories.AsNoTracking()
                .FirstOrDefaultAsync(item => item.Category == model.Category && item.Name == model.ProductName);
            if (product == null)
            {
                ModelState.AddModelError(string.Empty, "No inventory item matches that category and product name. Record Stock In for it first.");
                await transaction.RollbackAsync();
                await PopulateAsync(model);
                return View("Index", model);
            }

            var updated = await _context.Inventories
                .Where(item => item.Id == product.Id && item.Stock >= model.Quantity)
                .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.Stock, item => item.Stock - model.Quantity));
            if (updated != 1)
            {
                ModelState.AddModelError(string.Empty, "There is not enough stock available for that quantity.");
                await transaction.RollbackAsync();
                await PopulateAsync(model);
                return View("Index", model);
            }

            _context.StockMovements.Add(new StockMovement
            {
                InventoryId = product.Id,
                ProductName = product.Name,
                Category = product.Category,
                Quantity = model.Quantity,
                MovementType = "StockOut",
                Reason = model.Reason,
                Note = model.Note,
                UnitPrice = product.UnitPrice,
                CreatedAt = model.Date.Date.Add(DateTime.Now.TimeOfDay)
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            TempData["Success"] = "Stock out recorded and inventory updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Undo(int id, DateTime? date)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var movement = await _context.StockMovements.AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id && item.MovementType == "StockOut" && !item.IsVoided);
            if (movement == null)
            {
                await transaction.RollbackAsync();
                return NotFound();
            }

            var markedVoided = await _context.StockMovements
                .Where(item => item.Id == id && item.MovementType == "StockOut" && !item.IsVoided)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(item => item.IsVoided, true)
                    .SetProperty(item => item.VoidedAt, DateTime.Now));
            if (markedVoided != 1)
            {
                await transaction.RollbackAsync();
                return Conflict("This stock-out entry was already voided.");
            }

            var productUpdated = await _context.Inventories
                .Where(item => item.Id == movement.InventoryId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.Stock, item => item.Stock + movement.Quantity));
            if (productUpdated != 1)
            {
                await transaction.RollbackAsync();
                return Conflict("The related product could not be found; no inventory changes were saved.");
            }

            await transaction.CommitAsync();
            TempData["Success"] = "Stock-out entry voided; the quantity was restored.";
            return RedirectToAction(nameof(Index), new { date = (date ?? DateTime.Today).ToString("yyyy-MM-dd") });
        }

        private async Task PopulateAsync(StockOutViewModel model)
        {
            model.Categories = await GetCategoriesAsync();

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            var todaysMovements = _context.StockMovements
                .AsNoTracking()
                .Where(movement => movement.MovementType == "StockOut"
                    && !movement.IsVoided
                    && movement.CreatedAt >= today
                    && movement.CreatedAt < tomorrow);

            model.UnitsSoldToday = await todaysMovements
                .Where(movement => movement.Reason == "Sold")
                .SumAsync(movement => (int?)movement.Quantity) ?? 0;
            model.EstimatedSalesValueToday = await todaysMovements
                .Where(movement => movement.Reason == "Sold")
                .SumAsync(movement => (decimal?)(movement.Quantity * movement.UnitPrice)) ?? 0m;
            model.TransactionsToday = await todaysMovements.CountAsync();

            var selectedDay = model.FilterDate.Date;
            model.Movements = await _context.StockMovements
                .AsNoTracking()
                .Include(movement => movement.Inventory)
                .Include(movement => movement.Vendor)
                .Where(movement => movement.MovementType == "StockOut"
                    && movement.CreatedAt >= selectedDay
                    && movement.CreatedAt < selectedDay.AddDays(1))
                .OrderByDescending(movement => movement.CreatedAt)
                .ToListAsync();
        }

        private async Task<List<string>> GetCategoriesAsync()
        {
            var existingCategories = await _context.Inventories
                .Select(item => item.Category)
                .Distinct()
                .ToListAsync();
            return InventoryCategories.Defaults
                .Concat(existingCategories)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(category => category)
                .ToList();
        }
    }
}
