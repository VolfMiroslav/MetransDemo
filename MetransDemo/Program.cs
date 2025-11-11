using Microsoft.EntityFrameworkCore;
using MetransDemo.Controllers;
using MetransDemo.Models;

namespace MetransDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<MetransDemo.Models.MetransDemoContext>(options =>
                options.UseSqlServer(
                    "Server=MIREKVOLFPC\\SQLEXPRESS;Database=MetransDemoDb;Trusted_Connection=True;TrustServerCertificate=True;"
            ));

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<MetransDemoContext>();
                DataSeeder.SeedAllAsync(context).Wait();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            ;

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
