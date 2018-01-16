using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Users.Models;
using Users.Models.ViewModels.Users;
using System.Threading.Tasks;

namespace Users.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<AppUser> _userManager;

        public AdminController(UserManager<AppUser> userManager) => _userManager = userManager;

        public ViewResult Index() => View(_userManager.Users);

        public ViewResult Create() => View();

        [HttpPost]  
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                var appUser = new AppUser
                {
                    UserName = model.Name,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(appUser, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors) 
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);  
        }
    }
}