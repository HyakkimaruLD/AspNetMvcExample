using AspNetMvcExample.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AspNetMvcExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SiteContext _context;

        public HomeController(ILogger<HomeController> logger, SiteContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string searchText, string profession, string skill, string experienceRange, string ageRange)
        {
            var users = _context.UserInfos
                .Include(u => u.MainImageFile)
                .Include(u => u.UserSkills)
                    .ThenInclude(us => us.Skill)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                users = users.Where(u => u.Name.Contains(searchText));
            }

            if (!string.IsNullOrEmpty(profession))
            {
                users = users.Where(u => u.Profession == profession);
            }

            if (!string.IsNullOrEmpty(skill))
            {
                users = users.Where(u => u.UserSkills.Any(us => us.Skill.Title == skill));
            }

            if (!string.IsNullOrEmpty(experienceRange))
            {
                users = users.Where(u =>
                    (experienceRange == "0" && u.ExpirienseYears == 0) ||
                    (experienceRange == "1-2" && u.ExpirienseYears >= 1 && u.ExpirienseYears <= 2) ||
                    (experienceRange == "3-5" && u.ExpirienseYears >= 3 && u.ExpirienseYears <= 5) ||
                    (experienceRange == "5+" && u.ExpirienseYears > 5)
                );
            }

            if (!string.IsNullOrEmpty(ageRange))
            {
                var today = DateTime.Today;
                users = users.Where(u =>
                    (ageRange == "0-18" && (today.Year - u.Birthday.Year - (today < u.Birthday.AddYears(today.Year - u.Birthday.Year) ? 1 : 0)) <= 18) ||
                    (ageRange == "19-25" && (today.Year - u.Birthday.Year - (today < u.Birthday.AddYears(today.Year - u.Birthday.Year) ? 1 : 0)) >= 19 &&
                                            (today.Year - u.Birthday.Year - (today < u.Birthday.AddYears(today.Year - u.Birthday.Year) ? 1 : 0)) <= 25) ||
                    (ageRange == "26-35" && (today.Year - u.Birthday.Year - (today < u.Birthday.AddYears(today.Year - u.Birthday.Year) ? 1 : 0)) >= 26 &&
                                            (today.Year - u.Birthday.Year - (today < u.Birthday.AddYears(today.Year - u.Birthday.Year) ? 1 : 0)) <= 35) ||
                    (ageRange == "36+" && (today.Year - u.Birthday.Year - (today < u.Birthday.AddYears(today.Year - u.Birthday.Year) ? 1 : 0)) > 35)
                );
            }

            ViewBag.Professions = _context.UserInfos
                .Select(u => u.Profession)
                .Distinct()
                .ToList();

            ViewBag.Skills = _context.Skills
                .Select(s => s.Title)
                .Distinct()
                .ToList();

            return View(users.ToList());
        }





        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Hello()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
