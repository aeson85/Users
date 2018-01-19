using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Users.Models.ViewModels.Users;
using Microsoft.AspNetCore.Identity;
using Users.Models;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Users.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [AllowAnonymous]
        public ViewResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var appUser =  await _userManager.FindByEmailAsync(model.Email);
                if (appUser != null)
                {
                    await _signInManager.SignOutAsync();
                    var signInResult = await _signInManager.PasswordSignInAsync(appUser, model.Password, false, false);
                    if (signInResult.Succeeded)
                    {
                        return Redirect(returnUrl ?? "/");
                    }
                }
                ModelState.AddModelError("", "用户名或密码错误.");
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult GoogleLogin(string returnUrl)
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account", new 
            {
                ReturnUrl = returnUrl
            });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", returnUrl);
            return new ChallengeResult("Google", properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("Login");
            }
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }
            else
            {
                var user = new AppUser
                {
                    UserName = info.Principal.FindFirst(ClaimTypes.Email).Value,
                    Email = info.Principal.FindFirst(ClaimTypes.Email).Value
                };
                var identityResult = await _userManager.CreateAsync(user);
                if (identityResult.Succeeded)
                {
                    identityResult = await _userManager.AddLoginAsync(user, info);
                    if (identityResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, false);
                        return Redirect(returnUrl);
                    }
                }
                return AccessDenied();
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ViewResult AccessDenied() => View();
    }
}