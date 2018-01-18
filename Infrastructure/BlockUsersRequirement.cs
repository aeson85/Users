using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Users.Infrastructure
{
    public class BlockUsersRequirement : IAuthorizationRequirement
    {
        public string[] BlockUsers { get; set; }

        public BlockUsersRequirement(params string[] users) => this.BlockUsers = users;
    }

    public class BlockUsersHandler : AuthorizationHandler<BlockUsersRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BlockUsersRequirement requirement)
        {
            if (context.User.Identity != null && context.User.Identity.Name != null && !requirement.BlockUsers.Any(p => string.Equals(p, context.User.Identity.Name, StringComparison.OrdinalIgnoreCase)))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}