using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class SalesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
