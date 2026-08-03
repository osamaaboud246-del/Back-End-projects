using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb.Models;
using StudentPortalWeb.Services;
using System.Diagnostics;

namespace StudentPortalWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly StudentPortalContext _context;
        private readonly IOsamaStampService _stampA;
        private readonly IOsamaStampService _stampB;

        public HomeController(
            StudentPortalContext context,
            IOsamaStampService stampA,
            IOsamaStampService stampB)
        {
            _context = context;
            _stampA = stampA;
            _stampB = stampB;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Owner = _stampA.Owner;
            ViewBag.StampA = _stampA.Stamp;
            ViewBag.StampB = _stampB.Stamp;

            var students = await _context.Students
                .OrderBy(s => s.FullName)
                .ToListAsync();

            return View(students);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}