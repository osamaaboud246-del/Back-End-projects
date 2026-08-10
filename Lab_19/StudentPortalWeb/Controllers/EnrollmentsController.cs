using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortalWeb.Controllers
{
    // LAB 19 — Lab ID: 7 | MIN_GRADE_LAB = 2.5 | COURSE_COUNT = 3
    public class EnrollmentsController : Controller
    {
        private readonly StudentPortalContext _context;

        public EnrollmentsController(StudentPortalContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Students = await _context.Students.OrderBy(s => s.FullName).ToListAsync();

            ViewBag.Courses = await _context.Courses.ToListAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Enrollment enrollment)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Students = await _context.Students.OrderBy(s => s.FullName).ToListAsync();
                ViewBag.Courses = await _context.Courses.ToListAsync();
                return View(enrollment);
            }

            enrollment.EnrollmentDate = DateTime.Now;
            await _context.Enrollments.AddAsync(enrollment);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Enrollment created successfully.";
            return RedirectToAction("Details", "Students", new { id = enrollment.StudentId });
        }

        }
}