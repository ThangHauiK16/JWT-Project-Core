namespace JWT_Project_Core.Interface
{
    public interface IFileService
    {
        Task<string?> SaveImageAsync(IFormFile file);
        void DeleteImage(string filePath);
    }
}
