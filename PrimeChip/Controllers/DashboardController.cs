using Microsoft.AspNetCore.Mvc;
using PrimeChip.Data;

namespace PrimeChip.Controllers
{
    public class DashboardController : CheckController
    {
        public IActionResult Index()
        {
            return View();
        } 
    }
}
