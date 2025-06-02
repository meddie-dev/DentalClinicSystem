using CMS.Data;
using CMS.Models;
using Microsoft.AspNetCore.Identity;

namespace CMS.Services
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                logger.LogInformation("Note: Make sure the Database is created.");
                await context.Database.EnsureCreatedAsync();

                logger.LogInformation("Note: Seeding Inventory Items...");
                InventoryItemSeeder.SeedInventoryItems(context);

                logger.LogInformation("Note: Seeding Roles...");
                await AddRoleAsync(roleManager, "Admin");
                await AddRoleAsync(roleManager, "Staff");
                await AddRoleAsync(roleManager, "Doctor");

                logger.LogInformation("Note: Seeding Admin...");
                var adminEmail = "admin@email.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var adminUser = new Users
                    {
                        FullName = "Admin User",
                        UserName = adminEmail,
                        NormalizedEmail = adminEmail.ToUpper(),
                        Email = adminEmail,
                        NormalizedUserName = adminEmail.ToUpper(),
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        SecurityStamp = Guid.NewGuid().ToString(),

                    };

                    var result = await userManager.CreateAsync(adminUser, "Password123.");
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Note: Assigning Admin role to Admin Userr.");
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                    else
                    {
                        logger.LogError("Note: Failed to create Admin User. Errors: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }   
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static async Task AddRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    Console.WriteLine($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
                else
                {
                    Console.WriteLine($"Role '{roleName}' created successfully.");
                }
            }
            else
            {
                Console.WriteLine($"Role '{roleName}' already exists.");
            }
        }
    }
}
