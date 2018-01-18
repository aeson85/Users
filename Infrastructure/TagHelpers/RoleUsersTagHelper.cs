using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Identity;
using Users.Models;
using System.Collections.Generic;
using System.Linq;

namespace Users.Infrastructure.TagHelpers
{
    [HtmlTargetElement("td", Attributes="identity-role")]
    public class RoleUsersTagHelper : TagHelper
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleUsersTagHelper(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HtmlAttributeName("identity-role")]
        public string Role { get; set; }
        
        public override async void Process(TagHelperContext context, TagHelperOutput output) 
        {
            var resultLst = new List<string>();
            var identityRole = await _roleManager.FindByIdAsync(this.Role);
            if (identityRole != null)
            {
                foreach (var appUser in _userManager.Users)
                {
                    if(await _userManager.IsInRoleAsync(appUser, identityRole.Name))
                    {
                        resultLst.Add(appUser.UserName);
                    }
                }
            }
            output.Content.SetContent(resultLst.Count() == 0 ? "No Users" : string.Join(",", resultLst));
        }
    }
}