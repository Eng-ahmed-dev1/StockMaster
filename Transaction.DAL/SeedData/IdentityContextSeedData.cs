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
            var adminEmail = "ahmed@gmail.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new SystemUsers
                {
                    FullName = "Ahmed Alaa",
                    UserName = "Own:Ahmed",
                    Email = adminEmail,
                    PhoneNumber = "01226598971",
                    EmailConfirmed = true,
                    IsActive = true,
                    Gender = Gender.Male,
                    City = "Alexandria",
                    Country ="Egypt",
                    CreatedAt = DateTime.Now,
                    DateOfBirth = new DateTime(2000,11,30),
                };

                var adminResult = await userManager.CreateAsync(admin, "P@ssw0rd");
                if (adminResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}