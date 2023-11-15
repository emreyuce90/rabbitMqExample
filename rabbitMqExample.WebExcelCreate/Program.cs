using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using rabbitMqExample.WebExcelCreate.Models;
using rabbitMqExample.WebExcelCreate.Services;

namespace rabbitMqExample.WebExcelCreate {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options => {
                options.UseSqlServer(builder.Configuration.GetConnectionString("sqlServer"));
            });
            builder.Services.AddIdentity<IdentityUser,IdentityRole>(opt=> {
                opt.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>();
            
            
            #region RabbitMq
            builder.Services.AddSingleton(sp=>new ConnectionFactory { Uri = new Uri(builder.Configuration.GetConnectionString("rabbitMq")),DispatchConsumersAsync=true});
            builder.Services.AddSingleton<RabbitMqClientServices>();
            builder.Services.AddSingleton<RabbitMqPublisher>();
            #endregion
            
            
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}