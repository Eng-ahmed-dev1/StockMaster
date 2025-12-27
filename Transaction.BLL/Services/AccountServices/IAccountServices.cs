using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Transaction.BLL
{
    public interface IAccountServices
    {
        // Authentication
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<SignInResult> LoginAsync(LoginViewModel model);  
        Task LogoutAsync();
        // User Managment 
        Task<UserViewModel?> GetUserByIdAsync(string userId);
        Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordViewModel model);
        //User Roles 
        Task<IEnumerable<string>> GetUserRolesAsync(string userId);  
        Task<bool> IsInRoleAsync(string userId, string roleName);
        Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName);
        Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName);
        // Duplicated handling errors 
        Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsUserNameExistsAsync(string userName);
    }
}