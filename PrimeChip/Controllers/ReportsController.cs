using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult InventorySummary()
        {
            return View();
        }

        public IActionResult SalesReport()
        {
            return View();
        }

        public IActionResult TotalInventoryValue()
        {
            return View();
        }

    }
}
