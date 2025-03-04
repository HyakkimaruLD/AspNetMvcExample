using AspNetMvcExample.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetMvcExample.Areas.Auth.Models.Forms;
using AspNetMvcExample.Models.Services;

namespace AspNetMvcExample.Areas.Auth.Controllers
{
    [Area("Auth")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole<int>> roleManager;
        private readonly FileStorage fileStorage;
        private readonly SiteContext context;

        public RoleController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager, FileStorage fileStorage, SiteContext context)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.fileStorage = fileStorage;
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users.ToListAsync();
            var model = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                model.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Roles = roles.ToList()
                });
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var allRoles = await roleManager.Roles.Select(r => r.Name).ToListAsync();

            var model = new EditUserRolesForm
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                AvailableRoles = allRoles,
                SelectedRoles = userRoles.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserRolesForm form)
        {
            var user = await userManager.FindByIdAsync(form.UserId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            bool updated = false;

            if (!string.IsNullOrEmpty(form.Email) && form.Email != user.Email)
            {
                user.Email = form.Email;
                updated = true;
            }

            if (!string.IsNullOrEmpty(form.Phone) && form.Phone != user.PhoneNumber)
            {
                user.PhoneNumber = form.Phone;
                updated = true;
            }

            if (form.Image != null)
            {
                if (user.Image != null)
                {
                    await fileStorage.DeleteAsync(user.Image);
                    context.ImageFiles.Remove(user.Image);
                }

                user.Image = await fileStorage.SaveAsync(form.Image);
            }
            await context.SaveChangesAsync();

            if (updated)
            {
                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to update user details.");
                    return View(form);
                }
            }

            if (!string.IsNullOrEmpty(form.NewPassword))
            {
                await userManager.RemovePasswordAsync(user);
                var passwordResult = await userManager.AddPasswordAsync(user, form.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    ModelState.AddModelError("", "Failed to update password.");
                    return View(form);
                }
            }

            var currentRoles = await userManager.GetRolesAsync(user);
            var rolesToAdd = form.SelectedRoles.Except(currentRoles);
            var rolesToRemove = currentRoles.Except(form.SelectedRoles);

            await userManager.AddToRolesAsync(user, rolesToAdd);
            await userManager.RemoveFromRolesAsync(user, rolesToRemove);

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Failed to delete user.");
            }

            return RedirectToAction("Index");
        }
    }
}
