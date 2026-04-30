using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class VendorsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
