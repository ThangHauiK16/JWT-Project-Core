using JWT_Project_Core.DTO;

namespace JWT_Project_Core.Interface
{
    public interface IUserService
    {
        Task<PagedResult<UserDTO>> GetUsersAsync(int page, int pageSize, string? keyword = null);
        Task<UserDTO?> GetUserAsync(string username);
        Task<bool> UpdateUserAsync(string username, UserDTO dto);
        Task<bool> DeleteUserAsync(string username);
    }
}
