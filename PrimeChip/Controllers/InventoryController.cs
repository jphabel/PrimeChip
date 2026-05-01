using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class InventoryController : CheckController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
