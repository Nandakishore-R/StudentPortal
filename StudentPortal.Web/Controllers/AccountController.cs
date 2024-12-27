using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Web.Models;
using System.Diagnostics;

namespace StudentPortal.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await userManager.CreateAsync(user, model.Password);
                Debug.WriteLine($"Email: {model.Email}");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("List", "Students"); // Redirect after successful registration
                }
                else
                {
                    Debug.WriteLine("Goone");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    Debug.WriteLine($"Error: {error.Description}");
                }
            }

            return View(model); // If registration fails, return the view with errors
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByEmailAsync(model.Email);

                    if (await userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("AdminDashboard", "Dashboard");
                    }
                    else if (await userManager.IsInRoleAsync(user, "User"))
                    {
                        return RedirectToAction("UserDashboard", "Dashboard");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
