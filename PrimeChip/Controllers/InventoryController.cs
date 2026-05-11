using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class InventoryController : CheckController
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
