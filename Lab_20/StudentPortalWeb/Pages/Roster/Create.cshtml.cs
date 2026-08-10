using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentPortalWeb.Models;
using System.Threading.Tasks;

namespace StudentPortalWeb.Pages.Roster
{
    // LAB 20 — Lab ID: 29 | MIN_GPA_LAB = 1.5 | MAX_YEAR_LAB = 4
    // (a) Razor Pages uses the HTTP verb in the method name (OnGet, OnPost) on a single class, so they cannot share the same name, unlike MVC which distinguishes them using attributes.
    // (b) Making it a property automatically maintains its state across the page lifecycle and makes it directly accessible in the HTML view without passing it explicitly.
    // (c) Returning the view leaves the POST request in the browser; if the user presses F5, the browser resubmits the form and creates a duplicate row.
    // Part D (2): Pressing F5 does not insert a second row because RedirectToPage forces a new GET request, clearing the POST payload.
    // Part D (4): Deleting [BindProperty] causes the form to submit with a 200 OK status, but the Student object remains empty (null fields), failing validation and inserting nothing.

    public class CreateModel : PageModel
    {
        private readonly StudentPortalContext _context;

        public CreateModel(StudentPortalContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Student Student { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Student.Gpa < 1.5)
            {
                ModelState.AddModelError("Student.Gpa", "GPA must be at least 1.5 for this intake.");
            }

            if (Student.YearOfStudy > 4)
            {
                ModelState.AddModelError("Student.YearOfStudy", "Year of study may not exceed 4 for this intake.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _context.Students.AddAsync(Student);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{Student.FullName} was added the Razor Pages way";
            return RedirectToPage("./Index");
        }
    }
}
