using FuckingLectures.Data;
using FuckingLectures.Helpers;
using FuckingLectures.Models.Entities;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace FuckingLectures.Extensions
{
    public static class AppBuilderExtensions
    {
        // -------------------------------
        // SEED SUPER ADMIN
        // -------------------------------
        public static async Task<WebApplication> UseSeeder(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<AppData>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            var seeder = new Seeder(context, userManager, roleManager);

            await seeder.SeedSuperAdmin("MgOsama", "MgOsama1!");

            return app;
        }

        // -------------------------------
        // SEED ROLES SAFELY (ASYNC)
        // -------------------------------
        public static async Task<WebApplication> UseIdentitySeedRoles(
            this WebApplication app,
            params string[] roles)
        {
            using var scope = app.Services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
                }
            }

            return app;
        }

        // -------------------------------
        // AUTH MIDDLEWARE
        // -------------------------------
        public static WebApplication UseAuth(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }

        // -------------------------------
        // CUSTOM SWAGGER UI
        // -------------------------------
        public static WebApplication UseCustomSwagger(this WebApplication app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/app/swagger.json", "Lecures App API v1");
                options.SwaggerEndpoint("/swagger/dashboard/swagger.json", "Lecures Dashboard API v1");

                options.RoutePrefix = "swagger";

                // Custom UI
                options.InjectStylesheet("/swagger/swagger-dark.css");
                options.InjectJavascript("/swagger/theme-switcher.js");

                options.DocumentTitle = "Lecures";
                options.DocExpansion(DocExpansion.None);
                options.DisplayRequestDuration();

                // Removed (no longer supported)
                // options.EnableFilter();
                // options.EnableValidator();

                options.EnableDeepLinking();
                options.EnablePersistAuthorization();
                options.EnableTryItOutByDefault();
            });

            return app;
        }

        /*
        public static WebApplication UseFirbase(this WebApplication app)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile("firebase-adminsdk.json"),
            });



            return app;
        }*/

    }
}
