using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AspNetMvcExample.Models
{
    public class User : IdentityUser<int>
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        public virtual ImageFile? Image { get; set; }

        //public virtual ICollection<UserInfo> Infos { get; set;  }
    }
}
