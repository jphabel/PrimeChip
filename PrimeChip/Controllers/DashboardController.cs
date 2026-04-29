using Microsoft.AspNetCore.Mvc;
using PrimeChip.Data;

namespace PrimeChip.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            var user = HttpContext.Session.GetString("User");

            if (string.IsNullOrEmpty(user))
            {
                return RedirectToAction("Login", "Login");
            }
            return View();
        }
    }
}
