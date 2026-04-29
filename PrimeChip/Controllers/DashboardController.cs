using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
