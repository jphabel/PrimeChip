using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
