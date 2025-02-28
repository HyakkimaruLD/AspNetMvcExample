namespace AspNetMvcExample.Models;

public class UserSkill
{
    public int Id { get; set; }
    public virtual UserInfo UserInfo { get; set; } = null!;
    public virtual Skill Skill { get; set; } = null!;
    public int Level { get; set; }
}
