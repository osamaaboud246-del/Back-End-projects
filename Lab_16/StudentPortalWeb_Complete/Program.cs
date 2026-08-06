// =====================================================================
// StudentPortalWeb_Complete — FULL WORKING FALLBACK (Rule 20)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 16 — Routing: Custom Routes, Constraints, Attribute Routing
//
// Complete, correct, runnable version of everything taught live today.
// Matches Instructor_Guide_EN.md and Student_Guide.md exactly (Rule 15).
//
// Run this, and every one of these URLs works:
//
//   https://localhost:7019/                        the Session 15 page, untouched
//   https://localhost:7019/students                the roster
//   https://localhost:7019/students/3              one student
//   https://localhost:7019/students/year/2         one academic year
//   https://localhost:7019/students/honours/first  one honours band
//   https://localhost:7019/students/search?name=a  the query-string search
//
// And every one of these is refused by the ROUTE TABLE, before any
// controller action runs — watch the console: no [START] line is
// followed by a controller, and no action is entered:
//
//   https://localhost:7019/students/abc            id is not an integer
//   https://localhost:7019/students/year/7         year is outside 1-4
//   https://localhost:7019/students/honours/third  not a real band name
// =====================================================================

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentPortalWeb_Complete.Constraints;
using StudentPortalWeb_Complete.Models;
using System;

namespace StudentPortalWeb_Complete
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // =========================================================
            // PHASE ONE — WHAT CAN THIS APP DO?
            // =========================================================
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // BLOCK 4, TODO 1 — teaching the routing system a new word.
            //
            // ConstraintMap is a dictionary from the nickname you type
            // inside a route pattern to the class that implements it.
            // "int", "range" and "alpha" are already in this dictionary
            // when the app starts; Microsoft put them there exactly the
            // same way. Adding "honourband" does not extend routing with
            // a special case — it fills in one more row of a table that
            // was always meant to be filled in.
            //
            // Note we hand over the TYPE, not an instance. The framework
            // constructs the constraint itself, when it needs one. That
            // is the same "you do not call new, the framework does"
            // idea Session 15 spent a whole block on, appearing again in
            // a completely different corner of the framework.
            //
            // This must sit BEFORE builder.Build(): it is a service
            // registration, and Session 15 established that nothing can
            // be registered after the container is built.
            builder.Services.AddRouting(options =>
            {
                options.ConstraintMap.Add("honourband", typeof(HonourBandConstraint));
            });

            builder.Services.AddDbContext<StudentPortalContext>(options =>
            {
                options.UseSqlServer("Data Source=.;Initial Catalog=ITI_StudentPortal;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");
            });
            var app = builder.Build();
            // ↑↑↑ THE DIVIDING LINE. Above: what exists. Below: what runs.

            // =========================================================
            // PHASE TWO — HOW IS A REQUEST HANDLED?
            // =========================================================
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Session 15's middleware, unchanged, doing new work today.
            // It is registered BEFORE UseRouting, so it prints every path
            // the server receives — including the ones routing is about
            // to refuse. That is what makes "the route said no" visible
            // rather than merely asserted.
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"[START] Request path : {context.Request.Path}");
                await next.Invoke();
                Console.WriteLine($"[END] Request path : {context.Request.Path}");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseAuthentication();
            app.UseAuthorization();

            // =========================================================
            // THE ROUTE TABLE — read it top to bottom. This is the
            // application's public API surface, and it is now a design
            // decision rather than an accident of class names.
            //
            // ORDER MATTERS: routing walks this list from the top and
            // stops at the FIRST pattern that matches. Move the default
            // route to the top and /students/3 is read as
            // controller "students", action "3" — which does not exist,
            // so it 404s, while /students keeps working because Index is
            // the default action. Partial breakage like that is far
            // harder to debug than total breakage, which is exactly why
            // specific routes go above general ones.
            // =========================================================

            // BLOCK 2, TODO 2 — the list. One literal segment, no
            // parameters. The URL says nothing about our class names;
            // the defaults object holds that mapping privately, which is
            // what lets us rename the controller tomorrow without
            // breaking anybody's bookmark.
            app.MapControllerRoute(
                name: "studentsList",
                pattern: "students",
                defaults: new { controller = "Students", action = "Index" });

            // BLOCK 2, TODO 2 + BLOCK 3, TODO 3 — one student.
            // The :int is Block 3's addition. Without it, /students/abc
            // matches this route, model binding gives Details an id of 0,
            // the action runs a pointless database query and returns
            // NotFound. With it, routing refuses the URL before any of
            // that happens — and, critically, falls through to try the
            // NEXT route rather than failing outright. A constraint is a
            // matching rule, not a validation error.
            app.MapControllerRoute(
                name: "studentDetails",
                pattern: "students/{id:int}",
                defaults: new { controller = "Students", action = "Details" });

            // BLOCK 3, TODO 3 — one academic year.
            // Two constraints chained on one parameter: it must be a
            // whole number AND fall between 1 and 4 inclusive. Chaining
            // reads as AND, always. Note that range() alone would still
            // let a non-numeric through to be rejected later; stating
            // :int first makes the intent explicit and the failure
            // earlier.
            app.MapControllerRoute(
                name: "studentsByYear",
                pattern: "students/year/{year:int:range(1,4)}",
                defaults: new { controller = "Students", action = "ByYear" });

            // BLOCK 4, TODO 4 — one honours band, guarded by a
            // constraint that did not exist until we wrote it. To the
            // routing system this line is indistinguishable from the
            // three above it.
            app.MapControllerRoute(
                name: "studentHonours",
                pattern: "students/honours/{band:honourband}",
                defaults: new { controller = "Students", action = "Honours" });

            // Session 15's default route, unchanged, deliberately LAST.
            // It is the catch-all: anything the designed routes above did
            // not claim falls through to the old controller/action/id
            // shape, so /Home/Privacy still works and nothing that used
            // to be reachable stopped being reachable.
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
