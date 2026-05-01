using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class AlertsController : Controller
    {
        public IActionResult Index()
        {
            var user = HttpContext.Session.GetString("user");

            if (string.IsNullOrEmpty(user))
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}
