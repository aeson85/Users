
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Users.Models.ViewModels.Users
{
    public class RoleModificationModel
    {
        [Required]
        public string RoleName { get; set; }

        public string RoleId { get; set; }

        public IEnumerable<string> IdsToAdd { get; set; }

        public IEnumerable<string> IdsToDelete { get; set; }
    }
}