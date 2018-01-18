using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Users.Models;

namespace Users.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        private Task<AppUser> CurrentUser => _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

        public HomeController(UserManager<AppUser> userManager) => _userManager = userManager;

        [Authorize]
        public ViewResult Index() => View(GetData(nameof(Index)));

        [Authorize(Policy="DCUsers")]
        public ViewResult OtherAction() => View(nameof(Index), GetData(nameof(OtherAction)));
        
        [Authorize(Policy="NoYJ")]
        public ViewResult NoYjAction() => View(nameof(Index), GetData(nameof(NoYjAction)));

        [Authorize]
        public async Task<ViewResult> UserProps()
        {
            return View(await this.CurrentUser);
        }

        [HttpPost]
        [Authorize]     
        public async Task<IActionResult> UserProps([Required]Cities city, [Required]QualificationLevels qualifications)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await this.CurrentUser;
                appUser.City = city;
                appUser.Qualifications = qualifications;
                await _userManager.UpdateAsync(appUser);
                return RedirectToAction(nameof(Index));
            }
            return View(await this.CurrentUser);
        }

        private Dictionary<string, object> GetData(string actionName) => new Dictionary<string, object>
        {
            ["Action Name"] = actionName,
            ["User"] = HttpContext.User.Identity.Name,
            ["Authenticated"] = HttpContext.User.Identity.IsAuthenticated.ToString(),
            ["Auth Type"] = HttpContext.User.Identity.AuthenticationType,
            ["In Users Role"] = HttpContext.User.IsInRole("Users"),
            ["City"] = this.CurrentUser.Result.City,
            ["Qualification"] = this.CurrentUser.Result.Qualifications
        };
    }
}