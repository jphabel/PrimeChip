using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class SalesController : CheckController
    {
        public IActionResult Index() => RedirectToAction("Index", "StockOut");
    }
}