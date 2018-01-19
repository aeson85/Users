using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Users.Models;
using System;

namespace Users.Infrastructure
{
    public class DocumentAuthorizationRequirement : IAuthorizationRequirement
    {
        public bool AllowAuthors { get; set; }

        public bool AllowEditors { get; set; }
    }

    public class DocumentAuthorization : AuthorizationHandler<DocumentAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DocumentAuthorizationRequirement requirement)
        {
            var doc = context.Resource as ProtectedDocument;
            var userName = context.User?.Identity?.Name;
            var compare = StringComparison.OrdinalIgnoreCase;
            if (doc != null && !string.IsNullOrEmpty(userName))
            {
                if (requirement.AllowAuthors && string.Equals(doc.Author, userName, compare) || requirement.AllowEditors && string.Equals(doc.Editor, userName, compare))
                {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}