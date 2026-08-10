using Microsoft.EntityFrameworkCore;
using StudentPortalWeb.Models;
using StudentPortalWeb.Services;

namespace StudentPortalWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<StudentPortalContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Lifetime for Lab ID 29: Singleton
            builder.Services.AddSingleton<IOsamaStampService, OsamaStampService>();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value;
                Console.WriteLine($"[START] {path}");

                if (path != null && path.Contains("/audit-29"))
                {
                    Console.WriteLine($"[AUDIT] Osama Aboud saw a request for {path}");
                }

                await next();
                Console.WriteLine($"[END] {path}");
            });

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

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
