using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Users.Models;
using System.Linq;

namespace Users.Infrastructure
{
    public class CustomPasswordValidator : PasswordValidator<AppUser>
    {
        public CustomPasswordValidator(IdentityErrorDescriber errors = null) : base(errors)
        {
        }

        public override async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            var result = await base.ValidateAsync(manager, user, password);
            var errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();
            if (password.Contains(user.UserName))
            {
                errors.Add(new IdentityError 
                {
                    Description = "密码不能包含用户名"
                });
            }
            if (password.Contains("12345"))
            {
                errors.Add(new IdentityError
                {
                    Description = "密码不能包含连续数字"
                });
            }
            return errors.Count() == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}