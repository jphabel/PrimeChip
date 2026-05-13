using Microsoft.AspNetCore.Mvc;
using PrimeChip.Data;

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
            var user = HttpContext.Session.GetString("user");

            if (!string.IsNullOrEmpty(user))
            {
                HttpContext.Session.Clear();
            }
            return View(); 
        }
        public IActionResult TestHash()
        {
            var hash = BCrypt.Net.BCrypt.HashPassword("1234");
            return Content(hash);
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
        }
        [HttpPost] 
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            
            {
                HttpContext.Session.SetString("user", user.Email);
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Invalid credentials"; 
            return View("Index");
        }
    }
}
