using TgSeeker.Persistent;
using TgSeeker.Persistent.Repositiories;
using TgSeeker.Persistent.Sqlite.Repositiories;
using TgSeeker.Web.Services;

namespace TgSeeker.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await DbInitializer.InitializeAsync();

            var builder = WebApplication.CreateBuilder(args);

            {
                var tgSeekerService = new TgSeekerHostedService();
                builder.Services.AddSingleton<TgSeekerHostedService>(opts => tgSeekerService);
                builder.Services.AddHostedService<TgSeekerHostedService>(opts => tgSeekerService);
            }

            builder.Services.AddScoped<ISettingsRepository, SettingsRepository>();

            builder.Services.AddCors(options =>
            {
                var origins = new[] { builder.Configuration["clientUrlHttp"], builder.Configuration["clientUrlHttps"] };

                options.AddPolicy(name: "_myAllowSpecificOrigins",
                                  policy =>
                                  {
                                      policy.WithOrigins(origins);
                                      policy.AllowAnyMethod();
                                      policy.AllowAnyHeader();
                                  });
            });

            // Add services to the container.
            builder.Services.AddRazorPages();

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
            
            app.UseCors("_myAllowSpecificOrigins");

            app.UseAuthorization();

            app.MapControllers();
            app.MapRazorPages();

            app.Run();
        }
    }
}
