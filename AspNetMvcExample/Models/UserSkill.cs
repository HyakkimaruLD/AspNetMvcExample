using System.ComponentModel.DataAnnotations.Schema;

namespace AspNetMvcExample.Models;

public class UserSkill
{
    public int Id { get; set; }
    public int UserInfoId { get; set; }  // Добавляем свойство UserInfoId
    public virtual UserInfo UserInfo { get; set; } = null!;

    public int SkillId { get; set; } // Добавляем свойство SkillId
    public virtual Skill Skill { get; set; } = null!;
    public int Level { get; set; }
}
