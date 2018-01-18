using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Users.Models;
using System.Linq;

namespace Users.Infrastructure
{
    public class CustomNameValidator : UserValidator<AppUser>
    {
        public CustomNameValidator(IdentityErrorDescriber errors = null) : base(errors)
        {
        }

        public override async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var result = await base.ValidateAsync(manager, user);
            var errors = result.Succeeded ? new List<IdentityError>() : result.Errors.ToList();
            // if (!user.Email.ToLower().EndsWith("@example.com"))
            // {
            //     errors.Add(new IdentityError
            //     {
            //         Code = "EmailDomainError",
            //         Description = "Only example.com email addresses are allowed"
            //     });
            // }
            return errors.Count() == 0 ? IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}