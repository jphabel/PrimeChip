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

            return View();
        } 
    }
}
