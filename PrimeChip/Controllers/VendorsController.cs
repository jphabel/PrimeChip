using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class VendorsController : CheckController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
