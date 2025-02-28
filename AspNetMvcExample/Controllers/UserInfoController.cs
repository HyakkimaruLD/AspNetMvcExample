using AspNetMvcExample.Models;
using AspNetMvcExample.Models.Forms;
using AspNetMvcExample.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AspNetMvcExample.Controllers;

[Authorize]
public class UserInfoController(
    ILogger<UserInfoController> logger,
    SiteContext context,
    FileStorage fileStorage
    ) : Controller
{
    public IActionResult Index()
    {
        return View(
            context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .ToList()
            );
    }

    public IActionResult View(int id) 
    {
        return View(
            context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .First(x => x.Id == id)
            );
    }

    public IActionResult ViewPartial(int id) 
    {
        return View(
            "View",
            context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .First(x => x.Id == id)
            );
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new UserInfoForm(new UserInfo());
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] UserInfoForm form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var model = new UserInfo();
        form.Update(model);

        context.UserInfos.Add(model);

        if (form.Gallery != null)
        {
            var imageFiles = new List<ImageFile>();
            foreach (var item in form.Gallery)
            {
                var imageFile = await fileStorage.SaveAsync(item);
                imageFiles.Add(imageFile);
            }

            context.ImageFiles.AddRange(imageFiles);
            
            imageFiles.ForEach(x => model.ImageFiles.Add(x));


            if (imageFiles.Any())
            {
                model.MainImageFile = imageFiles.First();
            }
        }
        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        ViewData["id"] = id;

        var model = await context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .ThenInclude(x => x.ImageFile)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (model == null)
        {
            return NotFound();
        }

        var form = new UserInfoForm(model);

        var userSkills = model.UserSkills ?? new List<UserSkill>();

        var skills = await context.Skills
            .Include(x => x.ImageFile)
            .ToListAsync();

        var availableSkills = skills
            .Where(x => !userSkills.Select(us => us.Skill.Id).Contains(x.Id))
            .ToList();

        ViewData["userSkills"] = userSkills;
        ViewData["skills"] = skills;
        ViewData["availableSkills"] = availableSkills;

        return View(form);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] UserInfoForm form)
    {
        if (!ModelState.IsValid)
        {
            ViewData["id"] = id;
            return View(form);
        }

        var model = await context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (model == null)
        {
            return NotFound();
        }

        if (form.Gallery != null)
        {
            foreach (var item in form.Gallery)
            {
                var imageFile = await fileStorage.SaveAsync(item);
                model.ImageFiles.Add(imageFile);
            }
        }

        form.Update(model);

        await context.SaveChangesAsync();

        return RedirectToAction("Index");
    }


    public async Task<IActionResult> ChangeMainImage(int id, [FromQuery] int imageId)
    {
        var model = await context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .FirstAsync(x => x.Id == id);

        model.MainImageFile = model.ImageFiles.First(x => x.Id == imageId);
        await context.SaveChangesAsync();

        return Json(new { Ok = true });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var userSkill = await context.UserSkills.FirstAsync(x => x.Id == id);
        if (userSkill != null)
        {
            context.UserSkills.Remove(userSkill);
            await context.SaveChangesAsync();
        }
        return Json(new { Ok = true });
    }

    [HttpPost]
    public async Task<IActionResult> AddSkill(int id, [FromBody] UserSkillForm data)
    {
        var user = await context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .FirstAsync(x => x.Id == id);

        var skill = await context.Skills.FirstAsync(x => x.Id == data.SkillId);
        

        if (null != user.UserSkills.FirstOrDefault(x => x.Skill.Id == skill.Id))
        {
            Response.StatusCode = 400;
            return Json(new { Ok = false, Error = "Alredy exists" });
        }

        user.UserSkills.Add(new UserSkill
        {
            Level = data.Level,
            Skill = skill,
            UserInfo = user
        });

        await context.SaveChangesAsync();

        return Json(new { Ok = true });
    }


   

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var userInfo = await context.UserInfos.FindAsync(id);

        if (userInfo != null)
        {
            foreach (var image in userInfo.ImageFiles)
            {
                await fileStorage.DeleteAsync(image);
            }

            context.UserInfos.Remove(userInfo);
            await context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }



}
