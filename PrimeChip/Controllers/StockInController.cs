using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeChip.Data;
using PrimeChip.Models;

namespace PrimeChip.Controllers
{
    public class StockInController : CheckController
    {
        private readonly AppDbContext _context;

        public StockInController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await BuildViewModelAsync(new StockInViewModel());
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockInViewModel model)
        {
            model.ProductName = model.ProductName?.Trim() ?? string.Empty;
            model.Category = model.Category?.Trim() ?? string.Empty;
            var allowedCategories = await GetCategoriesAsync();
            if (!allowedCategories.Contains(model.Category, StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(nameof(model.Category), "Select a valid product category.");
            }

            if (!ModelState.IsValid)
            {
                await BuildViewModelAsync(model);
                return View("Index", model);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            var product = await _context.Inventories
                .FirstOrDefaultAsync(item => item.Category == model.Category && item.Name == model.ProductName);
            var vendorExists = await _context.Vendors.AnyAsync(vendor => vendor.Id == model.VendorId);
            if (!vendorExists)
            {
                ModelState.AddModelError(nameof(model.VendorId), "Select an existing vendor.");
                await transaction.RollbackAsync();
                await BuildViewModelAsync(model);
                return View("Index", model);
            }

            if (product == null)
            {
                if (string.IsNullOrWhiteSpace(model.SKU) || model.UnitPrice == null)
                {
                    ModelState.AddModelError(string.Empty, "For a new product, enter its SKU and unit price. Existing products are matched by category and name.");
                    await transaction.RollbackAsync();
                    await BuildViewModelAsync(model);
                    return View("Index", model);
                }

                product = new Inventory
                {
                    Name = model.ProductName,
                    Category = model.Category,
                    SKU = model.SKU.Trim(),
                    InitialStock = model.Quantity,
                    Stock = model.Quantity,
                    ReorderLevel = model.ReorderLevel,
                    UnitPrice = model.UnitPrice.Value,
                    CreatedAt = DateTime.Now
                };
                _context.Inventories.Add(product);
                await _context.SaveChangesAsync();
            }
            else
            {
                var updated = await _context.Inventories
                    .Where(item => item.Id == product.Id)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(item => item.Stock, item => item.Stock + model.Quantity));
                if (updated != 1)
                {
                    ModelState.AddModelError(string.Empty, "The selected product could not be updated.");
                    await transaction.RollbackAsync();
                    await BuildViewModelAsync(model);
                    return View("Index", model);
                }
            }

            _context.StockMovements.Add(new StockMovement
            {
                InventoryId = product.Id,
                ProductName = product.Name,
                Category = product.Category,
                VendorId = model.VendorId,
                Quantity = model.Quantity,
                MovementType = "StockIn",
                Note = model.Note,
                UnitPrice = product.UnitPrice,
                CreatedAt = model.Date.Date.Add(DateTime.Now.TimeOfDay)
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            TempData["Success"] = "Stock received and inventory updated.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<StockInViewModel> BuildViewModelAsync(StockInViewModel model)
        {
            model.Categories = await GetCategoriesAsync();
            model.Vendors = await _context.Vendors.OrderBy(vendor => vendor.vendorName).ToListAsync();
            model.RecentMovements = await _context.StockMovements
                .AsNoTracking()
                .Include(movement => movement.Inventory)
                .Include(movement => movement.Vendor)
                .Where(movement => movement.MovementType == "StockIn")
                .OrderByDescending(movement => movement.CreatedAt)
                .Take(25)
                .ToListAsync();
            return model;
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
