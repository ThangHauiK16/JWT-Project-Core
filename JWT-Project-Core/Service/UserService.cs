using JWT_Project_Core.Data;
using JWT_Project_Core.DTO;
using JWT_Project_Core.Interface;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace JWT_Project_Core.Service
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách user + phân trang
        public async Task<PagedResult<UserDTO>> GetUsersAsync(int page, int pageSize)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                var total = await query.CountAsync();
                var users = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserDTO
                    {
                        Username = u.Username,
                        Role = u.Role
                    })
                    .ToListAsync();

                Log.Information("Lấy danh sách user (page {Page}, size {PageSize}) thành công!", page, pageSize);

                return new PagedResult<UserDTO>(users, total, page, pageSize);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetUsersAsync: unexpected error");
                throw;
            }
        }

        // Lấy chi tiết user
        public async Task<UserDTO?> GetUserAsync(string username)
        {
            try
            {
                var user = await _context.Users.FindAsync(username);
                if (user == null)
                {
                    Log.Warning("Không tìm thấy user: {Username}", username);
                    return null;
                }

                Log.Information("Lấy thông tin user {Username} thành công", username);

                return new UserDTO
                {
                    Username = user.Username,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetUserAsync: unexpected error with username {Username}", username);
                throw;
            }
        }

        // Update user
        public async Task<bool> UpdateUserAsync(string username, UserDTO dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(username);
                if (user == null)
                {
                    Log.Warning("UpdateUserAsync: user {Username} không tồn tại", username);
                    return false;
                }

                if (!string.IsNullOrEmpty(dto.Password))
                {
                    user.Password = dto.Password;
                }

                user.Role = dto.Role;
                await _context.SaveChangesAsync();

                Log.Information("Cập nhật user {Username} thành công", username);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdateUserAsync: lỗi khi cập nhật user {Username}", username);
                throw;
            }
        }

        // Xoá user
        public async Task<bool> DeleteUserAsync(string username)
        {
            try
            {
                var user = await _context.Users.FindAsync(username);
                if (user == null)
                {
                    Log.Warning("DeleteUserAsync: user {Username} không tồn tại", username);
                    return false;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                Log.Information("Xóa user {Username} thành công", username);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeleteUserAsync: lỗi khi xóa user {Username}", username);
                throw;
            }
        }
    }
}
