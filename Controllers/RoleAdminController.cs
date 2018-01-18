using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Users.Models;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Users.Models.ViewModels.Users;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Users.Controllers
{
    [Authorize(Roles="Admins")]
    public class RoleAdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleAdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public ViewResult Index() => View(_roleManager.Roles);

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create([Required]string name)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                AddModelErrorFromResult(result);
            }
            return View(name);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([Required]string id)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    var result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    AddModelErrorFromResult(result);
                }
            }
            return View("Index", _roleManager.Roles);
        }

        public async Task<ViewResult> Edit([Required]string id)
        {
            List<AppUser> member = null;
            List<AppUser> nonMember = null;
            IdentityRole role = null;
            if (ModelState.IsValid)
            {
                role = await _roleManager.FindByIdAsync(id);
                if (role != null)
                {
                    List<AppUser> tmpLst;
                    foreach (var appUser in _userManager.Users)
                    {
                        tmpLst = await _userManager.IsInRoleAsync(appUser, role.Name) ? member = member ?? new List<AppUser>() : nonMember = nonMember ?? new List<AppUser>();
                        tmpLst.Add(appUser);
                    }
                }
            }
            return View(new RoleEditModel
            {
                Role = role,
                Members = member,
                NonMembers = nonMember
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleModificationModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.IdsToAdd?.Count() > 0)
                {
                    foreach (var appUserId in model.IdsToAdd)
                    {
                        var appUser = await _userManager.FindByIdAsync(appUserId);
                        if (appUser != null)
                        {
                            var addResult = await _userManager.AddToRoleAsync(appUser, model.RoleName);
                            if (!addResult.Succeeded)
                            {
                                AddModelErrorFromResult(addResult);
                            }
                        }
                    }
                }
                
                if (model.IdsToDelete?.Count() > 0)
                {
                    foreach (var appUserId in model.IdsToDelete)
                    {
                        var appUser = await _userManager.FindByIdAsync(appUserId);
                        var removeReslt = await _userManager.RemoveFromRoleAsync(appUser, model.RoleName);
                        if (!removeReslt.Succeeded)
                        {
                            AddModelErrorFromResult(removeReslt);
                        }
                    }
                }
            }
            if (ModelState.IsValid)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return await Edit(model.RoleId);
            }
        }

        private void AddModelErrorFromResult(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}