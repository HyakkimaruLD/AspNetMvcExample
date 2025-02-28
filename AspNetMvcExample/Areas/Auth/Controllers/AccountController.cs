using AspNetMvcExample.Areas.Auth.Models.Forms;
using AspNetMvcExample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AspNetMvcExample.Areas.Auth.Controllers
{
    [Area("Auth")]
    public class AccountController(UserManager<User> userManager, SignInManager<User> signInManager, SiteContext context) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegisterForm form)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            if (await userManager.FindByEmailAsync(form.Login) != null)
            {
                ModelState.AddModelError(nameof(form.Login), "User already exists");
                return View(form);
            }

            var user = new User
            {
                Email = form.Login,
                UserName = form.Login
            };

            var result = await userManager.CreateAsync(user, form.Password);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Registration failed");
                return View(form);
            }

            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home", new { Area = "" });
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterForm());
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginForm());
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginForm form)
        {
            if (!ModelState.IsValid)
            {
                return View(form);
            }

            var user = await userManager.FindByEmailAsync(form.Login);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View(form);
            }

            var result = await signInManager.PasswordSignInAsync(user, form.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt");
                return View(form);
            }


            await signInManager.SignInWithClaimsAsync(user, true, [
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("Avatar", user.Image?.Src ?? "")
                ]);

            return RedirectToAction("Index", "Home", new { Area = "" });
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { Area = "" });
        }





        //-------------------


        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordForm());
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromForm] ChangePasswordForm form)
        {
            if (!ModelState.IsValid || form.NewPassword != form.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match");
                return View(form);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var result = await userManager.ChangePasswordAsync(user, form.OldPassword, form.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(form);
            }

            await signInManager.RefreshSignInAsync(user);
            return RedirectToAction("Index", "Profile", new { Area = "Auth" });
        }
    }
}
