using JWT_Project_Core.DTO;

namespace JWT_Project_Core.Interface
{
    public interface IAuthService
    {
        //Task<string?> LoginAsync(LoginDTO login);
        Task<object?> LoginAsync(LoginDTO login);
        Task<string?> RefreshTokenAsync(string refreshToken);
        Task<bool> RemoveRefreshTokenAsync(string refreshToken);
        Task<string> RegisterAsync(RegisterDTO newUser);
    }
}
