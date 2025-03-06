using System.ComponentModel.DataAnnotations;

namespace AspNetMvcExample.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        public int UserInfoId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string Comment { get; set; } = null!;
        public string UserName { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual UserInfo UserInfo { get; set; } = null!;
    }
}
