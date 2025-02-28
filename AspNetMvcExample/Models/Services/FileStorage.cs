using System.Reflection;

namespace AspNetMvcExample.Models.Services;

public class FileStorage(IWebHostEnvironment environment)
{
    public async Task<ImageFile> SaveAsync(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        var filename = Guid.NewGuid().ToString() + extension;
        var dir1 = filename[0].ToString();
        var dir2 = filename[1].ToString();
        Directory.CreateDirectory(Path.Combine(environment.WebRootPath, "uploads", dir1, dir2));
        var fullFilename = Path.Combine(environment.WebRootPath, "uploads", dir1, dir2, filename);
        using var fs = new FileStream(fullFilename, FileMode.Create);
        await file.CopyToAsync(fs);

        return new ImageFile
        {
            Filename = string.Join("/", new List<string> { dir1, dir2, filename }),
            OriginalFilename = file.FileName,
        };
    }

    public async Task DeleteAsync(ImageFile file)
    {
        var fullFilename = Path.Combine(environment.WebRootPath, "uploads", file.Filename);
        if (File.Exists(fullFilename))
        {
            await Task.Run(() => File.Delete(fullFilename));
        }
    }


}
