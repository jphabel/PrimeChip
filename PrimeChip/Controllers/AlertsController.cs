using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class AlertsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
