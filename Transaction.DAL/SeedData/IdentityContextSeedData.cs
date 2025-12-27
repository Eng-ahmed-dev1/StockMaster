using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Transaction.DAL
{
    public static class IdentityContextSeedData
    {
        public static async Task<bool> SeedData(
          UserManager<SystemUsers> userManagers,
          RoleManager<IdentityRole> roleManagers)
        {
            try
            {
                var hasUsers = userManagers.Users.Any();
                var hasRoles = roleManagers.Roles.Any();

                if (hasUsers && hasRoles) return false;

                // ================= ROLES =================
                if (!hasRoles)
                {
                    var roles = new List<IdentityRole>
                    {
                        new() { Name = "Admin", NormalizedName = "ADMIN" },
                        new() { Name = "User", NormalizedName = "USER" }
                    };

                    foreach (var role in roles)
                    {
                        if (!await roleManagers.RoleExistsAsync(role.Name!))
                        {
                            await roleManagers.CreateAsync(role);
                        }
                    }
                }

                // ================= USERS =================
                if (!hasUsers)
                {
                    var admin = new SystemUsers
                    {
                        FullName = "Ahmed Alaa",
                        UserName = "DevAhmed",
                        Email = "ahmed@gmail.com",
                        PhoneNumber = "01030939232",
                        EmailConfirmed = true,
                        IsActive = true
                    };

                    var adminResult = await userManagers.CreateAsync(admin, "P@ssw0rd");
                    if (adminResult.Succeeded)
                    {
                        await userManagers.AddToRoleAsync(admin, "Admin");
                    }

                    var normalUser = new SystemUsers
                    {
                        FullName = "Mohamed",
                        UserName = "DevMohamed",
                        Email = "mohaed@gmail.com",
                        PhoneNumber = "01282845813",
                        EmailConfirmed = true,
                        IsActive = true
                    };

                    var userResult = await userManagers.CreateAsync(normalUser, "P@ssw0rd");
                    if (userResult.Succeeded)
                    {
                        await userManagers.AddToRoleAsync(normalUser, "User");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
                return false;
            }
        }
    }
}