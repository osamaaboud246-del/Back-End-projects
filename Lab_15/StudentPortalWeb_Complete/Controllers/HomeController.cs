// =====================================================================
// HomeController — COMPLETE / FALLBACK VERSION (Rule 20)
// Session 15 — Block 3, TODO 4-6
//
// This is the smallest honest proof that Dependency Injection worked.
// There is no `new` in this file. There is no connection string in this
// file. There is no mention of SQL Server in this file. And yet Index()
// reads the database.
//
// ⚠️ We are NOT learning Controllers today. Session 17 does that
//    properly. This controller exists to make DI visible.
// =====================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb_Complete.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortalWeb_Complete.Controllers
{
    public class HomeController : Controller
    {
        // TODO 4 (done). readonly, because it is assigned exactly once,
        // in the constructor, and must never be reassigned afterwards.
        // The leading underscore is the .NET convention marking a field
        // that arrived from outside rather than being built in here.
        private readonly StudentPortalContext _context;

        // TODO 5 (done). Read this constructor as a sentence:
        // "I cannot do my job without a StudentPortalContext."
        //
        // Nothing in this class satisfies that requirement. The
        // framework does — it reads this parameter list at startup, looks
        // up what was registered in Program.cs, builds one, and passes it
        // in. This is CONSTRUCTOR INJECTION, and it is the form of DI
        // you will use for the rest of your career.
        //
        // Notice what this makes possible that yesterday's `new` did
        // not: to test this controller you hand it a context pointed at
        // a throwaway database. The controller cannot tell and does not
        // care. That is the payoff of Session 11's "depend on the
        // abstraction, not on the concrete thing you happen to have."
        public HomeController(StudentPortalContext context)
        {
            _context = context;
        }

        // TODO 6 (done). async Task<IActionResult>, for exactly the same
        // reason Main became async Task in Session 14: the moment you
        // await something, the signature has to admit it.
        public async Task<IActionResult> Index()
        {
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
