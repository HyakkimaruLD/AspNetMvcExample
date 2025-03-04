using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AspNetMvcExample.Areas.Auth.Models.Forms
{
    public class EditUserRolesForm
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string? NewPassword { get; set; }

        public List<string> AvailableRoles { get; set; } = new();
        public List<string> SelectedRoles { get; set; } = new();
        public IFormFile? Image { get; set; }
    }
}
