using Microsoft.AspNetCore.Http;

namespace OurTube.Application.Services;

public static class LocalFilesService
{
    public static async Task<string> SaveFileAsync(IFormFile file, string directory, string fileName)
    {
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var filePath = Path.Combine(directory, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        return filePath;
    }
}