using Microsoft.AspNetCore.Mvc;

namespace PrimeChip.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            var user = HttpContext.Session.GetString("user");

            if (string.IsNullOrEmpty(user))
            {
                return RedirectToAction("Index","Login");
            }
            return View();
        }
    }
}
