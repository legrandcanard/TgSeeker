using TgSeeker.Persistent;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.Persistent.Sqlite.Repositiories;
using TgSeeker.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TgSeeker.Web.Data;
using TgSeeker.Web.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics;

namespace TgSeeker.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("ApplicationIdentityContextConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationIdentityContextConnection' not found.");

            builder.Services.AddDbContext<ApplicationIdentityContext>(options => options.UseSqlite(connectionString));

            builder.Services.AddControllers();

            builder.Services.AddDefaultIdentity<TgsUser>(options => {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.User.RequireUniqueEmail = false;

                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddUserManager<TgsUserManager>()
            .AddSignInManager()
            .AddEntityFrameworkStores<ApplicationIdentityContext>();

            {
                var tgSeekerService = new TgSeekerHostedService();
                builder.Services.AddSingleton<TgSeekerHostedService>(opts => tgSeekerService);
                builder.Services.AddHostedService<TgSeekerHostedService>(opts => tgSeekerService);
            }

            builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                //options.ExpireTimeSpan = TimeSpan.FromDays(10);
                //options.Cookie.MaxAge = TimeSpan.FromDays(10);
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

                //options.LoginPath = "/Identity/Account/Login";
                //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = (context) =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                await context.Response.WriteAsJsonAsync(new { error = exception.Message });
            }));

            app.MapControllers();
            app.MapFallbackToFile("index.html");

            await DbInitializer.InitializeAsync();
            using (var scope = app.Services.CreateScope())
            {
                var dbIdentityContext = scope.ServiceProvider.GetService<ApplicationIdentityContext>();
                await dbIdentityContext.Database.MigrateAsync();

                var userManager = scope.ServiceProvider.GetService<TgsUserManager>();
                var passwordHasher = scope.ServiceProvider.GetService<IPasswordHasher<TgsUser>>();

                var userName = builder.Configuration["adminUsername"];

                var user = await userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    user = new TgsUser
                    {
                        UserName = userName,
                        NormalizedEmail = userName.ToUpperInvariant(),
                    };
                    user.PasswordHash = passwordHasher.HashPassword(user, builder.Configuration["adminUserPassword"]);

                    await userManager.CreateAsync(user);
                }
            }

            app.Run();
        }
    }
}
