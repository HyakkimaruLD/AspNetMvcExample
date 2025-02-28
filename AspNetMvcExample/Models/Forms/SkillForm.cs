using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AspNetMvcExample.Models;

namespace AspNetMvcExample.Models.Forms;

public class SkillForm
{
    public SkillForm() { }
    public SkillForm(Skill model) { 
        Title = model.Title;
        Color = model.Color;
    }

    public void Update(Skill model)
    {
        model.Title = Title;
        model.Color = Color;
    }

    [DisplayName("Назва")]
    [Required(ErrorMessage = "Це поле обов'язкове")]
    [MinLength(2, ErrorMessage = "Мінімум 2 символи")]
    public string Title { get; set; } = null!;

    [DisplayName("Колір")]
    [Required(ErrorMessage = "Це поле обов'язкове")]
    public string Color { get; set; } = null!;

    [DisplayName("Аватарка")]
    public IFormFile? Image { get; set; }
}
