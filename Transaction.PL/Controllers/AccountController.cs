using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Transaction.BLL;

namespace Transaction.PL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountServices _accServices;

        public AccountController(IAccountServices accountServices)
        {
            _accServices = accountServices;
        }

        [HttpGet]
        [AllowAnonymous]  
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel registerView)
        {
            if (!ModelState.IsValid)
            {
                return View(registerView);
            }

            var result = await _accServices.RegisterAsync(registerView);

            if (result.Succeeded)
            {
                TempData["Success"] = "Account created successfully!";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, $"Error: {error.Description}");

            return View(registerView);
        }

        [HttpGet]
        [AllowAnonymous] 
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult>Login(LoginViewModel loginView, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(loginView);
            }

            var result = await _accServices.LoginAsync(loginView);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("ShowAllTransactions", "Transaction");  
            }

            if (result.IsLockedOut)
                ModelState.AddModelError(string.Empty, "Account locked.");
            else
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            return View(loginView);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _accServices.GetUserByIdAsync(userId!);

            if (user is null)
                return NotFound();

            return View(user);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _accServices.ChangePasswordAsync(userId!, model);

            if (result.Succeeded)
            {
                TempData["Success"] = "Password changed successfully!";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accServices.LogoutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}