using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeChip.Data;
using PrimeChip.Models;

namespace PrimeChip.Controllers
{
    public class AlertsController : CheckController
    {
        private readonly AppDbContext _context;

        public AlertsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var alerts = await _context.Inventories
                .AsNoTracking()
                .Where(item => item.Stock <= item.ReorderLevel)
                .OrderBy(item => item.Stock)
                .ThenBy(item => item.Name)
                .ToListAsync();

            return View(alerts);
        }
    }
}
