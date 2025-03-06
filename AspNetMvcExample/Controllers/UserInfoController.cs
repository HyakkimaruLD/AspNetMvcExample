using AspNetMvcExample.Models;
using AspNetMvcExample.Models.Forms;
using AspNetMvcExample.Models.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace AspNetMvcExample.Controllers;
//[Authorize(Roles = "Admin, User")]
[Route(template: "user-infos-data/[action]/{id:int?}")]
public class UserInfoController : Controller
{
    private readonly ILogger<UserInfoController> _logger;
    private readonly SiteContext _context;
    private readonly FileStorage _fileStorage;
    private readonly UserManager<User> _userManager;

    public UserInfoController(
        ILogger<UserInfoController> logger,
        SiteContext context,
        FileStorage fileStorage,
        UserManager<User> userManager
    )
    {
        _logger = logger;
        _context = context;
        _fileStorage = fileStorage;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        var userInfos = isAdmin
            ? await _context.UserInfos.Include(x => x.User).Include(x => x.MainImageFile).Include(x => x.ImageFiles).ToListAsync()
            : await _context.UserInfos.Where(x => x.UserId == user.Id).Include(x => x.MainImageFile).Include(x => x.ImageFiles).ToListAsync();

        return View(userInfos);
    }

    public async Task<IActionResult> View(int id)
    {
        var userInfo = await _context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .Include(x => x.Reviews)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (userInfo == null || (!await IsAuthorized(userInfo)))
        {
            return Forbid();
        }

        return View(userInfo);
    }

    public async Task<IActionResult> Create()
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

        var user = await _userManager.GetUserAsync(User);
        var model = new UserInfo { UserId = user.Id };
        form.Update(model);

        _context.UserInfos.Add(model);

        if (form.Gallery != null)
        {
            var imageFiles = new List<ImageFile>();
            foreach (var item in form.Gallery)
            {
                var imageFile = await _fileStorage.SaveAsync(item);
                imageFiles.Add(imageFile);
            }

            _context.ImageFiles.AddRange(imageFiles);
            imageFiles.ForEach(x => model.ImageFiles.Add(x));

            if (imageFiles.Any())
            {
                model.MainImageFile = imageFiles.First();
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet(template: "edit-info")]
    public async Task<IActionResult> Edit(int id)
    {
        var userInfo = await _context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .ThenInclude(x => x.ImageFile)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (userInfo == null || (!await IsAuthorized(userInfo)))
        {
            return Forbid();
        }

        ViewData["id"] = id;
        var form = new UserInfoForm(userInfo);
        var userSkills = userInfo.UserSkills ?? new List<UserSkill>();
        var skills = await _context.Skills.Include(x => x.ImageFile).ToListAsync();
        var availableSkills = skills.Where(x => !userSkills.Select(us => us.Skill.Id).Contains(x.Id)).ToList();

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

        var userInfo = await _context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (userInfo == null || (!await IsAuthorized(userInfo)))
        {
            return Forbid();
        }

        if (form.Gallery != null)
        {
            foreach (var item in form.Gallery)
            {
                var imageFile = await _fileStorage.SaveAsync(item);
                userInfo.ImageFiles.Add(imageFile);
            }
        }

        form.Update(userInfo);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ChangeMainImage(int id, [FromQuery] int imageId)
    {
        var userInfo = await _context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .Include(x => x.ImageFiles)
            .Include(x => x.MainImageFile)
            .FirstAsync(x => x.Id == id);

        if (userInfo == null || (!await IsAuthorized(userInfo)))
        {
            return Forbid();
        }

        userInfo.MainImageFile = userInfo.ImageFiles.First(x => x.Id == imageId);
        await _context.SaveChangesAsync();

        return Json(new { Ok = true });
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var userSkill = await _context.UserSkills.FirstAsync(x => x.Id == id);
        var userInfo = await _context.UserInfos.FirstAsync(x => x.Id == userSkill.UserInfoId);

        if (userSkill != null && (await IsAuthorized(userInfo)))
        {
            _context.UserSkills.Remove(userSkill);
            await _context.SaveChangesAsync();
        }
        return Json(new { Ok = true });
    }

    [HttpPost]
    public async Task<IActionResult> AddSkill(int id, [FromBody] UserSkillForm data)
    {
        var userInfo = await _context.UserInfos
            .Include(x => x.UserSkills)
            .ThenInclude(x => x.Skill)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (userInfo == null || (!await IsAuthorized(userInfo)))
        {
            return Forbid();
        }

        var skill = await _context.Skills.FirstOrDefaultAsync(x => x.Id == data.SkillId);

        if (skill == null)
        {
            return NotFound();
        }

        if (userInfo.UserSkills.Any(x => x.Skill.Id == skill.Id))
        {
            Response.StatusCode = 400;
            return Json(new { Ok = false, Error = "Already exists" });
        }

        userInfo.UserSkills.Add(new UserSkill
        {
            Level = data.Level,
            Skill = skill,
            UserInfo = userInfo
        });

        await _context.SaveChangesAsync();
        return Json(new { Ok = true });
    }

    [HttpPost]
    public async Task<IActionResult> AddReview(int id, [FromBody] ReviewForm reviewForm)
    {
        var user = await _userManager.GetUserAsync(User);
        var userInfo = await _context.UserInfos.FirstOrDefaultAsync(x => x.Id == id);

        if (userInfo == null)
        {
            return NotFound();
        }

        var review = new Review
        {
            UserInfoId = id,
            UserId = user.Id,
            UserName = user.FullName,
            Comment = reviewForm.Comment,
            Rating = reviewForm.Rating
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        return Json(new { Ok = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var userInfo = await _context.UserInfos.FindAsync(id);

        if (userInfo != null && (await IsAuthorized(userInfo)))
        {
            foreach (var image in userInfo.ImageFiles)
            {
                await _fileStorage.DeleteAsync(image);
            }

            _context.UserInfos.Remove(userInfo);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }

    private async Task<bool> IsAuthorized(UserInfo userInfo)
    {
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        return isAdmin || userInfo.UserId == user.Id;
    }
}