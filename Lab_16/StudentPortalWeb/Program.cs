// LAB 16 — Lab ID: 7 | MAX_YEAR = 4 | MIN_GPA = 3.0 | INTAKE_CODE = itiB
// The default route sits at the bottom of the table because routing evaluates patterns sequentially from top to bottom, so placing a catch-all default route at the top would match almost every URL and prevent specific custom routes below it from ever being reached.

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StudentPortalWeb.Constraints;
using StudentPortalWeb.Models;
using System;

namespace StudentPortalWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<StudentPortalContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddRouting(options =>
            {
                options.ConstraintMap.Add("honourBand", typeof(HonourBandConstraint));
                options.ConstraintMap.Add("intakecode", typeof(IntakeCodeConstraint));
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                Console.WriteLine($"[START] Request path : {context.Request.Path}");
                await next.Invoke();
                Console.WriteLine($"[END] Request path : {context.Request.Path}");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "roster",
                pattern: "roster",
                defaults: new { controller = "Students", action = "Index" });

            app.MapControllerRoute(
                name: "studentsList",
                pattern: "students",
                defaults: new { controller = "Students", action = "Index" });

            app.MapControllerRoute(
                name: "studentsDetails",
                pattern: "students/{id:int}",
                defaults: new { controller = "Students", action = "Details" });

            app.MapControllerRoute(
                name: "studentsByYear",
                pattern: "students/year/{year:int:range(1,4)}",
                defaults: new { controller = "Students", action = "ByYear" });

            app.MapControllerRoute(
                name: "studentsTop",
                pattern: "students/top/{count:int:range(1,4)}",
                defaults: new { controller = "Students", action = "Top" });

            app.MapControllerRoute(
                name: "studentsHonours",
                pattern: "students/honours/{band:honourBand}",
                defaults: new { controller = "Students", action = "Honours" });

            app.MapControllerRoute(
                name: "studentsIntake",
                pattern: "students/intake/{code:intakecode}",
                defaults: new { controller = "Students", action = "Intake" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}