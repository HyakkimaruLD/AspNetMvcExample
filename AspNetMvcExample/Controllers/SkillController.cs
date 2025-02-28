using AspNetMvcExample.Models;
using AspNetMvcExample.Models.Forms;
using AspNetMvcExample.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetMvcExample.Controllers;

[Authorize]
public class SkillController(
    ILogger<SkillController> logger,
    SiteContext context,
    FileStorage fileStorage
    ) : Controller
{

    public IActionResult Index()
    {
        return View(
            context.Skills
            .Include(x => x.ImageFile)
            .ToList()
            );
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var skill = await context.Skills.Include(x => x.ImageFile).FirstOrDefaultAsync(x => x.Id == id);

        if (skill == null)
        {
            return NotFound();
        }

        if (skill.ImageFile != null)
        {
            await fileStorage.DeleteAsync(skill.ImageFile); 
            context.ImageFiles.Remove(skill.ImageFile);
        }
        context.Skills.Remove(skill);
        await context.SaveChangesAsync();

        return RedirectToAction("Index"); 
    }

    [HttpGet]
    public IActionResult Create()
    {
        var model = new SkillForm(new Skill());
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] SkillForm form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }
        var model = new Skill();
        form.Update(model);

        if (form.Image != null)
        {
            model.ImageFile = await fileStorage.SaveAsync(form.Image);
        }

        context.Skills.Add(model);
        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }


    [HttpGet]
    public IActionResult Edit(int id)
    {
        ViewData["id"] = id;
        return View(new SkillForm(context.Skills.First(x => x.Id == id)));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromForm] SkillForm form)
    {
        if (!ModelState.IsValid)
        {
            ViewData["id"] = id;
            return View(form);
        }

        var model = context.Skills.First(x => x.Id == id);

        if (form.Image != null)
        {
            if (model.ImageFile != null)
            {
                fileStorage.DeleteAsync(model.ImageFile);
                context.ImageFiles.Remove(model.ImageFile);
            }
            model.ImageFile = await fileStorage.SaveAsync(form.Image);
        }

        form.Update(model);
        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
