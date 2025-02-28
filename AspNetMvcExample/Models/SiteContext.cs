using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetMvcExample.Models;

public class SiteContext: IdentityDbContext<User, IdentityRole<int>, int>
{
    public SiteContext(DbContextOptions options): base(options) { }

    public virtual DbSet<UserInfo> UserInfos { get; set; }
    public virtual DbSet<Skill> Skills { get; set; }
    public virtual DbSet<UserSkill> UserSkills { get; set; }
    public virtual DbSet<ImageFile> ImageFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ImageFile>()
            .HasMany(x => x.UserInfos)
            .WithMany(x => x.ImageFiles)
            .UsingEntity<Dictionary<string, string>>(
            "ImageFileUserInfo", 
            x => x.HasOne<UserInfo>().WithMany().HasForeignKey("UserInfoId"),
            x => x.HasOne<ImageFile>().WithMany().HasForeignKey("ImageFileId")
            );


        modelBuilder.Entity<UserInfo>()
            .HasOne<ImageFile>(x => x.MainImageFile);
            //.WithOne()
            //.HasForeignKey("MainImageFileId");
    }
}
