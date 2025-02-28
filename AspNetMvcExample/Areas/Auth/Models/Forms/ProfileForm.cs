namespace AspNetMvcExample.Areas.Auth.Models.Forms
{
    public class ProfileForm
    {
        public IFormFile? Image { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
    }
}
