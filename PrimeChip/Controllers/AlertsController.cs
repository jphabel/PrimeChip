using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class AlertsController : CheckController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
