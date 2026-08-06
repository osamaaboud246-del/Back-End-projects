// =====================================================================
// StudentPortalWeb_Complete — FULL WORKING FALLBACK (Rule 20)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 15 — ASP.NET MVC Overview, Dependency Injection, Middleware
//
// Complete, correct, runnable version of everything taught live today.
// Matches Instructor_Guide_EN.md and Student_Guide.md exactly (Rule 15).
//
// Run this and load https://localhost:7019/ and you should see the four
// Session 14 students rendered as an HTML table, with [START]/[END]
// lines appearing in the console for the page AND for every CSS and JS
// file the browser fetches afterwards.
// =====================================================================

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            // BLOCK 3, TODO 3 — wiring the context through DI.
            //
            // AddDbContext does three things in one call:
            //   1. registers StudentPortalContext so anything can ask for it,
            //   2. registers it with a SCOPED lifetime — one instance per
            //      HTTP request — which we did not have to specify and
            //      must not override,
            //   3. builds the DbContextOptions<StudentPortalContext> that
            //      the constructor in Models/StudentPortalContext.cs now
            //      demands, using the lambda below.
            //
            // The connection string lives HERE now, not inside the
            // context. That single move is what lets the same context
            // class point at a real database in production and a
            // throwaway one in a test, with no edit to the class itself.
            //
            // It is still a hardcoded literal, which is genuinely bad
            // practice — Session 19 moves it into appsettings.json. It
            // is spelled out in full today so every part is visible.
            builder.Services.AddDbContext<StudentPortalContext>(options =>
            {
                options.UseSqlServer(
                    "Server=.;Database=ITI_StudentPortalDB_EF;" +
                    "Trusted_Connection=True;TrustServerCertificate=True;");
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

            // BLOCK 4, TODO 8 — our own checkpoint, first in the hallway.
            //
            // It is registered FIRST deliberately, so it sees every
            // request before UseStaticFiles gets a chance to answer one
            // and short-circuit the rest of the pipeline. Move this
            // below UseStaticFiles and the CSS/JS requests vanish from
            // the log — which is itself a useful thing to demonstrate.
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"[START] Request path: {context.Request.Path}");

                // Opens the door to the next checkpoint. Without this
                // line the request stops here permanently: no exception,
                // no log, no response — just a browser spinning.
                await next.Invoke();

                // Runs on the way back OUT, after the controller and the
                // view have already finished. Every middleware gets two
                // turns: once inbound, once outbound.
                Console.WriteLine($"[END] Request path: {context.Request.Path}");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
