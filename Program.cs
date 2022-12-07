using Flower_site.DAL;
using FlowerSite.Areas.admin.Data;
using FlowerSite.Data;
using FlowerSite.Models.IdentityModels;
using FlowerSite.Services;
using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FlowerSite
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddMvc();
            builder.Services.AddLanguageService();
            builder.Services.AddControllersWithViews().AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            builder.Services.AddDbContext<AppDbContext>(
                opt => opt.UseSqlServer(builder.Configuration
                .GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.SignIn.RequireConfirmedEmail = false;
                opt.User.RequireUniqueEmail = true;

                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;

            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<LocalizedIdentityErrorDescriber>();

            builder.Services.AddSession(x => x.IdleTimeout = TimeSpan.FromMinutes(10));
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddTransient<IEmailService, EmailManager>();
            Constans.RootPath = builder.Environment.WebRootPath;
            var app = builder.Build();

            app.UseStatusCodePagesWithReExecute("/ErrorPage/Error1", "?code={0}");


            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();
            var locOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=dashboard}/{action=Index}/{id?}");

                app.MapControllerRoute(
                "default",
                "{controller=home}/{action=index}/{id?}"
                );
            });

            app.Run();
        }
    }
}