using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
