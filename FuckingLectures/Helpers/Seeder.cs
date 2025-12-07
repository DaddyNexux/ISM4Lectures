using Microsoft.AspNetCore.Identity;
using FuckingLectures.Data;
using FuckingLectures.Models.Entities;



namespace FuckingLectures.Helpers;

public class Seeder
{
    private readonly AppData _masterDbContext;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public Seeder(AppData masterDbContext, UserManager<User> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _masterDbContext = masterDbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedSuperAdmin(string email, string password)
    {
        // Check if the SuperAdmin user already exists
        var superAdmin = await _userManager.FindByEmailAsync(email);
        if (superAdmin == null)
        {
            // Create the SuperAdmin user
            superAdmin = new User
            {
                UserName = email,
                Email = email,
                FullName = "Super Admin",
                PhoneNumber = "7819535938",

            };

            var result = await _userManager.CreateAsync(superAdmin, password);
            if (result.Succeeded)
            {
                // Check if the "Admin" role exists, if not, create it
                var roleExist = await _roleManager.RoleExistsAsync(Roles.SuperAdmin);
                if (!roleExist)
                {
                    
                    throw new Exception("Failed to create 'Admin' role.");
                    
                }

                // Add the SuperAdmin user to the "Admin" role
                await _userManager.AddToRoleAsync(superAdmin, Roles.SuperAdmin);
            }
            else
            {
                throw new Exception("Failed to create SuperAdmin user.");
            }
        }
    }



}
