using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Transaction.DAL
{
    public static class IdentityContextSeedData
    {
        public static async Task<bool> SeedData(
            UserManager<SystemUsers> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            try
            {
                // ================= ROLES =================
                await SeedRolesAsync(roleManager);

                // ================= USERS =================
                await SeedUsersAsync(userManager);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding data: {ex.InnerException?.Message ?? ex.Message}");
                return false;
            }
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new List<string> { "Admin", "Supplier" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper()
                    });
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<SystemUsers> userManager)
        {
            // ========== Admin User ==========
            var adminEmail = "ahmed@gmail.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new SystemUsers
                {
                    FullName = "Ahmed Alaa",
                    UserName = "DevAhmed",
                    Email = adminEmail,
                    PhoneNumber = "01030939232",
                    EmailConfirmed = true,
                    IsActive = true,
                    Gender = Gender.Male,

                };

                var adminResult = await userManager.CreateAsync(admin, "P@ssw0rd");
                if (adminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // ========== Normal User ==========
            var userEmail = "mohamed@gmail.com"; 
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var normalUser = new SystemUsers
                {
                    FullName = "Mohamed",
                    UserName = "DevMohamed",
                    Email = userEmail,
                    PhoneNumber = "01282845813",
                    EmailConfirmed = true,
                    IsActive = true,
                    Gender = Gender.Male,
                };

                var userResult = await userManager.CreateAsync(normalUser, "P@ssw0rd");
                if (userResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "Supplier");
                }
            }
        }
    }
}