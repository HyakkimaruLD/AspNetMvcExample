namespace AspNetMvcExample.Areas.Auth.Models.Forms
{
    public class UserRolesViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
