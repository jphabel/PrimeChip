using Microsoft.AspNetCore.Mvc;
using PrimeChip.Data;
using PrimeChip.Models;

namespace PrimeChip.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        } 

        [HttpGet]
        public IActionResult Index()
        {
            return View(); 
        }

        [HttpPost] 
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.email == email);

            if (user != null)
            {
                HttpContext.Session.SetString("user", user.email);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Invalid credentials"; 
            return View("Index");
        }
    }
}
