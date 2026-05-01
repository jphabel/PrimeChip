using Microsoft.AspNetCore.Mvc;
using PrimeChip.Data;


namespace PrimeChip.Controllers
{
    public class DashboardController : CheckController
    {
        public IActionResult Index()
        {
            var TotalValue = _context.Inventories;
            return View();
        } 
    }
}
