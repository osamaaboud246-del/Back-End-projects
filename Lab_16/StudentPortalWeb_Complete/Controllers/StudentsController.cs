// =====================================================================
// StudentsController — FULL WORKING FALLBACK (Rule 20)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 16 — Routing
//
// ⚠️ WE ARE STILL NOT LEARNING CONTROLLERS TODAY.
//    Session 15 borrowed HomeController to PROVE that Dependency
//    Injection had delivered something real. Today we borrow this one
//    for the same kind of reason: a route has to point AT something, and
//    an empty room proves nothing. Session 17 is the session that builds
//    controllers properly — action results, model binding, validation,
//    all of it.
//
// Everything except the [Route]/[FromQuery] pair at the bottom is
// carried-forward skill (Rule 39): a
// constructor-injected context from Session 15, and EF Core queries from
// Session 14. It is all real, working code, pre-typed, and it compiles
// and runs before a single TODO is done. Not one line of it is today's
// topic.
//
// TODAY'S TOPIC IS THE URL, NOT THE METHOD. Every action below is
// currently reachable only through the default route inherited from
// Session 15 — /Students/Details/3 and friends. By the end of Block 4
// the same methods will answer /students/3, /students/year/2 and
// /students/honours/first, without one line inside them changing.
// =====================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb_Complete.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortalWeb_Complete.Controllers
{
    public class StudentsController : Controller
    {
        // Session 15, unchanged: the field, the constructor, the
        // injection. Nobody writes `new StudentPortalContext()` here.
        private readonly StudentPortalContext _context;

        public StudentsController(StudentPortalContext context)
        {
            _context = context;
        }

        // Answers the students list route (TODO 2).
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .OrderBy(s => s.FullName)
                .ToListAsync();

            return View(students);
        }

        // Answers the student detail route (TODO 2, constrained in TODO 3).
        //
        // The `id` parameter is filled in from ROUTE DATA — the routing
        // system pulls the value out of the URL segment and hands it to
        // this method by matching the segment's name to the parameter's
        // name. Rename either one and the value silently becomes 0.
        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
            {
                // The route matched, the action ran, the row did not
                // exist. That is a genuinely different failure from a URL
                // the route table refused, and Block 3 makes you tell
                // them apart from the console log alone.
                return NotFound();
            }

            return View(student);
        }

        // Answers the by-year route (TODO 3).
        public async Task<IActionResult> ByYear(int year)
        {
            var students = await _context.Students
                .Where(s => s.YearOfStudy == year)
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Year"] = year;

            return View(students);
        }

        // Answers the honours route (TODO 4), guarded by the constraint
        // you write in TODO 5.
        //
        // Read the guard order carefully. Reached through the HONOURS
        // route, `band` is already guaranteed to be one of three words,
        // because the constraint refused everything else — so the final
        // `else` below is not a silent catch-all for garbage; the garbage
        // never got this far. But this action is ALSO reachable through
        // the inherited default route, as /Students/Honours, where no
        // constraint applies at all. Hence the guard clause first.
        public async Task<IActionResult> Honours(string band)
        {
            // Guard clause before work — and note that this is NOT redundant
            // with the route constraint, even though the constraint makes it
            // unreachable through the honours route. The default route can
            // still reach this action as /Students/Honours with no band at
            // all, and an action must never assume it was only ever called
            // the way you intended. Constraints filter URLs; guard clauses
            // protect behaviour. Both, always.
            if (string.IsNullOrWhiteSpace(band))
            {
                return NotFound();
            }

            IQueryable<Student> query = _context.Students;

            if (string.Equals(band, "first", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(s => s.Gpa >= 3.5);
            }
            else if (string.Equals(band, "second", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(s => s.Gpa >= 3.0 && s.Gpa < 3.5);
            }
            else
            {
                query = query.Where(s => s.Gpa < 3.0);
            }

            var students = await query
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Band"] = band.ToLowerInvariant();

            return View(students);
        }

        // BLOCK 5, TODO 6 — the other kind of routing, and the other
        // way a URL carries information.
        //
        // [Route] is ATTRIBUTE ROUTING: the address is written on the
        // action itself instead of in Program.cs. The moment an action
        // carries its own address it stops answering the conventional
        // routes entirely — /Students/Search no longer works, only
        // /students/search does. That is not a quirk to work around; it
        // is the guarantee that makes attribute routing worth using: the
        // address is exactly what the attribute says, and nothing in
        // Program.cs can silently change it.
        //
        // [FromQuery] states plainly that this value arrives in the
        // QUERY STRING, not in a route segment. The code works without
        // it — model binding would find it anyway — so this attribute
        // buys nothing at runtime and everything at reading time. Route
        // data identifies WHICH resource (/students/3); the query string
        // refines HOW you want it (?name=nada&sort=gpa). A search term
        // is not an identity, so it does not belong in the path.
        [Route("students/search")]
        public async Task<IActionResult> Search([FromQuery] string name)
        {
            IQueryable<Student> query = _context.Students;

            // Guard clause before work: an empty search box is not an
            // error, it just means "no filter". Passing null straight
            // into Contains would throw instead.
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(s => s.FullName.Contains(name));
            }

            var students = await query
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Name"] = name;

            return View(students);
        }
    }
}
