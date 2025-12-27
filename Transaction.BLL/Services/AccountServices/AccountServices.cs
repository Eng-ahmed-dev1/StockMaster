using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Transaction.DAL;

namespace Transaction.BLL
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<SystemUsers> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<SystemUsers> _signInManager;
        private readonly IMapper _mapper;

        public AccountServices(
         UserManager<SystemUsers> userManager,
         SignInManager<SystemUsers> signInManager,
         RoleManager<IdentityRole> roleManager,
         IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterViewModel model)
        {
            if (await IsEmailExistsAsync(model.Email))
            {
                return IdentityResult.Failed(
                    new IdentityError { Description = "Email already Exist " }
                );
            }
            if (await IsUserNameExistsAsync(model.UserName))
            {
                return IdentityResult.Failed(
                   new IdentityError { Description = "The User Name already Exist " }
                );
            }
            var user = _mapper.Map<SystemUsers>(model);
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                if (await _roleManager.RoleExistsAsync("User"))
                {
                    result = await _userManager.AddToRoleAsync(user, "User");
                }
            }
            return result;
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.EmailOrUserName);
            if (user is null)
            {
                user = await _userManager.FindByNameAsync(model.EmailOrUserName);
            }

            if (user is null)
                return SignInResult.Failed;

            if (user.IsActive == false || user.IsDeleted == false)
                return SignInResult.Failed;

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!, model.Password, model.RememberMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }
            return result;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserViewModel?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return null;

            if (user.IsDeleted)
                return null;

            var User = _mapper.Map<UserViewModel>(user);

            User.Roles = await _userManager.GetRolesAsync(user);
            return User;

        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordViewModel model)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return IdentityResult.Failed(
                    new IdentityError { Description = "This User is not found " }
                );

            if (user.IsDeleted)
                return IdentityResult.Failed(
                     new IdentityError { Description = "This User account is deleted" }
                );


            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

            return result;
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByNameAsync(userId);

            if (user is null)
                return new List<string>();

            return await _userManager.GetRolesAsync(user);
        }

        public async Task<bool> IsInRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return false;

            return await _userManager.IsInRoleAsync(user, roleName);

        }


        public async Task<IdentityResult> AddUserToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return IdentityResult.Failed(
                new IdentityError { Description = "This User is Not exist" }
                );


            if (!await _roleManager.RoleExistsAsync(roleName))
                return IdentityResult.Failed(
                    new IdentityError { Description = $"{roleName} is not exist " }

                );

            if (await _userManager.IsInRoleAsync(user, roleName))
                return IdentityResult.Failed(

                    new IdentityError { Description = $"The {user.FullName} is already in {roleName}" }
                );


            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);


            if (user is null)
                return IdentityResult.Failed(
                new IdentityError { Description = "This User is Not exist" }
                );


            if (!await _roleManager.RoleExistsAsync(roleName))
                return IdentityResult.Failed(
                    new IdentityError { Description = $"{roleName} is not exist " }
                );

            if (!await _userManager.IsInRoleAsync(user, roleName))
                return IdentityResult.Failed(
                new IdentityError
                {
                    Description = $"The {user.FullName} is not in the role {roleName}"
                });

            return await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        public async Task<bool> IsUserNameExistsAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return user != null;
        }
    }
}