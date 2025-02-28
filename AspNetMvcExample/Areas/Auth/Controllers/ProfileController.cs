using AspNetMvcExample.Areas.Auth.Models.Forms;
using AspNetMvcExample.Models;
using AspNetMvcExample.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AspNetMvcExample.Areas.Auth.Controllers
{
    [Area("Auth")]
    [Authorize]
    public class ProfileController(
        UserManager<User> userManager,
        SiteContext context,
        FileStorage fileStorage,
        SignInManager<User> signInManager
        ) : Controller
    {
        private async Task<User> GetCurrentUserAsync()
        {
            var id = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            return await context.Users
                .Include(x => x.Image)
                .FirstAsync(x => x.Id == id);
        }

        public async Task<IActionResult> Index()
        {
            //var user = await context.Users
            //    .Include(x => x.Image)
            //    .FirstAsync(x => x.UserName == User.Identity.Name);

            //var id = int.Parse(User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
            //var user = await context.Users
            //    .Include(x => x.Image)
            //    .FirstAsync(x => x.Id == id);

            var user = await GetCurrentUserAsync(); 
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await GetCurrentUserAsync();




            return View( new ProfileForm
            {
                FullName = user.FullName,
                Phone = user.PhoneNumber
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] ProfileForm form)
        {
            if(!ModelState.IsValid)
            {
                return View(form);
            }
            var model = await GetCurrentUserAsync();
            model.FullName = form.FullName;
            model.PhoneNumber = form.Phone;

            if (form.Image != null)
            {
                if (model.Image != null)
                {
                    fileStorage.DeleteAsync(model.Image);
                    context.ImageFiles.Remove(model.Image);
                }
                model.Image = await fileStorage.SaveAsync(form.Image);
            }
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }



        //-------------------------------------

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            await signInManager.SignOutAsync();
            var result = await userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to delete account");
                return RedirectToAction("Profile");
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}
