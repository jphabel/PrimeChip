using Microsoft.AspNetCore.Mvc;
using PrimeChip.Data;

namespace PrimeChip.Controllers
{
    public class DashboardController : Controller
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
