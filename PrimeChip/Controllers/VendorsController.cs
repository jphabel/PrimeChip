using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PrimeChip.Data;
using PrimeChip.Models;

namespace PrimeChip.Controllers
{
    public class VendorsController : CheckController
    {
        private readonly AppDbContext _context;
        public VendorsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var suppliers = _context.Vendors.ToList();
            return View(suppliers);
        }

        [HttpPost]
        public IActionResult Create(Vendor item)
        {
            _context.Vendors.Add(item);
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Log this to see the actual database error
                Console.WriteLine(ex.InnerException?.Message);
                // If there's another level deep:
                Console.WriteLine(ex.InnerException?.InnerException?.Message);
            }
            return RedirectToAction("Index");
        }

    }
}
