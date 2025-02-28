namespace AspNetMvcExample.Models;

public class ImageFile
{
    public int Id { get; set; }
    public string OriginalFilename { get; set; } = null!;
    public string Filename { get; set; } = null!;
    public string Src => "/uploads/" + Filename;

    public virtual ICollection<UserInfo> UserInfos { get; set; } = null!;

}
