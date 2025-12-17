using AutoMapper;
using AutoMapper.QueryableExtensions;
using ClosedXML.Excel;
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

              
                user.IsDeleted = true;            
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                Log.Information("Soft delete user {Username} thành công", username);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeleteUserAsync error {Username}", username);
                throw;
            }
        }

        public async Task<byte[]> ExportUsersToExcelAsync(string? keyword = null)
        {
            try
            {
                var query = _context.Users
                    .Where(u => !u.IsDeleted)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query = query.Where(u => u.Username.Contains(keyword));
                }

                var users = await query
                    .OrderBy(u => u.Username)
                    .ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Users");

                worksheet.Cell(1, 1).Value = "Username";
                worksheet.Cell(1, 2).Value = "Email";
                worksheet.Cell(1, 3).Value = "Role";
                worksheet.Cell(1, 4).Value = "Created At";
                worksheet.Cell(1, 5).Value = "Is Deleted";

                var headerRange = worksheet.Range(1, 1, 1, 5);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;

                int row = 2;
                foreach (var user in users)
                {
                    worksheet.Cell(row, 1).Value = user.Username;
                    worksheet.Cell(row, 2).Value = user.Email;
                    worksheet.Cell(row, 3).Value = user.Role.ToString();
                    worksheet.Cell(row, 4).Value = user.CreatedAt.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(row, 5).Value = user.IsDeleted ? "Yes" : "No";
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);

                Log.Information("Xuất Excel danh sách user thành công");

                return stream.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ExportUsersToExcelAsync error");
                throw;
            }
        }
    }
}
