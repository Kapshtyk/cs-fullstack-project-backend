using Ecommerce.Services.Common.Interfaces;

namespace Ecommerce.Infrastructure.Services
{
    public class FileService(IWebHostEnvironment environment) : IFileService
    {
        private readonly IWebHostEnvironment _environment = environment;

        public async Task<string> SaveFileAsync(IFormFile file, string prefix)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file.");
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

            var uploadsFolderPath = Path.Combine(_environment.WebRootPath, prefix);

            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(prefix, uniqueFileName);
        }

        public async Task DeleteFileAsync(string path)
        {
            var filePath = Path.Combine(_environment.WebRootPath, path);

            if (File.Exists(filePath))
            {
                try
                {
                    await Task.Run(() => File.Delete(filePath));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}