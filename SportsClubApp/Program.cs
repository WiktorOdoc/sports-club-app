using Microsoft.EntityFrameworkCore;
using SportsClubApp.Data;
using SportsClubApp.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace SportsClubApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            string provider = builder.Configuration["DataProvider"]
    ?? throw new InvalidOperationException(
        "Nie określono DataProvider w appsettings.json"
    );

            provider = provider.ToUpperInvariant();

            var assembly = typeof(IRiderRepository).Assembly;

            string repositorySuffix = provider switch
            {
                "SQL" => "Sql",
                "FILE" => "File",
                _ => throw new InvalidOperationException(
                    $"Nieobsługiwany DataProvider: {provider}"
                )
            };

            var riderRepositoryType = assembly.GetType(
                $"SportsClubApp.Data.RiderRepository{repositorySuffix}"
            );

            var teamRepositoryType = assembly.GetType(
                $"SportsClubApp.Data.TeamRepository{repositorySuffix}"
            );

            if (riderRepositoryType == null ||
                !typeof(IRiderRepository).IsAssignableFrom(riderRepositoryType))
            {
                throw new InvalidOperationException(
                    "Nie znaleziono poprawnego repozytorium zawodników."
                );
            }

            if (teamRepositoryType == null ||
                !typeof(ITeamRepository).IsAssignableFrom(teamRepositoryType))
            {
                throw new InvalidOperationException(
                    "Nie znaleziono poprawnego repozytorium drużyn."
                );
            }

            if (provider == "SQL")
            {
                string connectionString =
                    builder.Configuration.GetConnectionString("DefaultConnection")!;

                builder.Services.AddDbContext<ApplicationDbContext>(
                    options => options.UseSqlite(connectionString)
                );
            }

            builder.Services.AddScoped(
                typeof(IRiderRepository),
                serviceProvider =>
                    (IRiderRepository)ActivatorUtilities.CreateInstance(
                        serviceProvider,
                        riderRepositoryType
                    )
            );

            builder.Services.AddScoped(
                typeof(ITeamRepository),
                serviceProvider =>
                    (ITeamRepository)ActivatorUtilities.CreateInstance(
                        serviceProvider,
                        teamRepositoryType
                    )
            );

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.UseAntiforgery();

            app.MapStaticAssets();

            app.MapRazorComponents<Components.App>()
            .AddInteractiveServerRenderMode();


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
