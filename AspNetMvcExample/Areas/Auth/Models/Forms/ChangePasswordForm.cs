using System.ComponentModel.DataAnnotations;

namespace AspNetMvcExample.Areas.Auth.Models.Forms
{
    public class ChangePasswordForm
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}