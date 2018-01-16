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
        private IUserValidator<AppUser> _userValidator;
        private IPasswordValidator<AppUser> _passValidator;
        private IPasswordHasher<AppUser> _passHasher;

        public AdminController(UserManager<AppUser> userManager, IUserValidator<AppUser> userValidator, IPasswordValidator<AppUser> passValidator, IPasswordHasher<AppUser> passHasher)
        {
            _userManager = userManager;
            _userValidator = userValidator;
            _passValidator = passValidator;
            _passHasher = passHasher;
        }

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

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser != null)
            {
                var result = await _userManager.DeleteAsync(appUser);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorToModelState(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found");
            }
            return View("Index", _userManager.Users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser != null)
            {
                return View(appUser);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string email, string password)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser != null)
            {
                appUser.Email = email;
                var userValiResult = await _userValidator.ValidateAsync(_userManager, appUser);
                IdentityResult passValiResult = null;
                if (!string.IsNullOrEmpty(password))
                {
                    passValiResult = await _passValidator.ValidateAsync(_userManager, appUser, password);
                    if (passValiResult.Succeeded)
                    {
                        appUser.PasswordHash = _passHasher.HashPassword(appUser, password);
                    }
                    else{
                        AddErrorToModelState(passValiResult);
                    }
                }
                if (userValiResult.Succeeded && (passValiResult?.Succeeded ?? true))
                {
                    await _userManager.UpdateAsync(appUser);
                    return RedirectToAction("Index");
                }
            }
            else
            {
                ModelState.AddModelError("", "User not found");
            }
            return View(appUser);
        }

        private void AddErrorToModelState(IdentityResult result)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}