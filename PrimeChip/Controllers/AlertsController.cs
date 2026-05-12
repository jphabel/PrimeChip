using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeChip.Data;

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


    }
}
