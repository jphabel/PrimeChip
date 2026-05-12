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

            return View();
        } 
    }
}
