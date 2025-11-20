using JWT_Project_Core.Interface;

namespace JWT_Project_Core.Service
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string?> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            if (string.IsNullOrEmpty(_env.WebRootPath))
                throw new InvalidOperationException("WebRootPath is null. Cannot save image.");

            string folderPath = Path.Combine(_env.WebRootPath, "images", "books");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"{Guid.NewGuid()}_{file.FileName}";
            string fullPath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/images/books/{fileName}";
        }

        public void DeleteImage(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(_env.WebRootPath))
                return;

            string fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }

    }
}
