using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeChip.Data;
using PrimeChip.Models;

namespace PrimeChip.Controllers
{
    public class ProfileController : CheckController
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("user");

            var user = _context.Users
                .FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View(user);
        }

        [HttpPost]
        public IActionResult Update(User model, IFormFile profilePhoto)
        {
            var email = HttpContext.Session.GetString("user");

            var user = _context.Users
                .FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return RedirectToAction("Index", "Login");
            }

    
            user.FullName = model.FullName;
            user.Username = model.Username;

            if (!string.IsNullOrWhiteSpace(model.Email) && model.Email != user.Email)
            {
                user.Email = model.Email;
                HttpContext.Session.SetString("user", model.Email);
            }

            if (profilePhoto != null && profilePhoto.Length > 0)
            {
                string folder = Path.Combine(_environment.WebRootPath, "uploads");

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(profilePhoto.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    profilePhoto.CopyTo(stream);
                }

                user.ProfileImage = "/uploads/" + fileName;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}