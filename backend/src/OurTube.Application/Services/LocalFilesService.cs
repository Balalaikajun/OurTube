using Microsoft.AspNetCore.Http;

namespace OurTube.Application.Services
{
    public class LocalFilesService
    {
        public async Task<string> SaveFileAsync(IFormFile file, string directory, string fileName)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string filePath = Path.Combine(directory, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
            return filePath;
        }
    }
}
