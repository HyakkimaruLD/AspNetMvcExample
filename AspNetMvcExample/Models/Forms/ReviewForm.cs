using System.ComponentModel.DataAnnotations;

namespace AspNetMvcExample.Models.Forms
{
    public class ReviewForm
    {
        [Required]
        public string Comment { get; set; } = null!;

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
