using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        private readonly IMapper _mapper;

        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        
        public async Task<PagedResult<UserDTO>> GetUsersAsync(int page, int pageSize, string? keyword = null)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(u => u.Username.Contains(keyword));
                }

                var total = await query.CountAsync();

                var users = await query
                    .OrderBy(u => u.Username)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ProjectTo<UserDTO>(_mapper.ConfigurationProvider) 
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

                return _mapper.Map<UserDTO>(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetUserAsync: unexpected error {Username}", username);
                throw;
            }
        }

       
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

              
                _mapper.Map(dto, user);


                await _context.SaveChangesAsync();

                Log.Information("Cập nhật user {Username} thành công", username);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdateUserAsync error {Username}", username);
                throw;
            }
        }

      
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
                Log.Error(ex, "DeleteUserAsync error {Username}", username);
                throw;
            }
        }
    }
}
