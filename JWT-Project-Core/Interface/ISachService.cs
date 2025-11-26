using JWT_Project_Core.DTO;

namespace JWT_Project_Core.Interface
{
    public interface ISachService
    {
        Task<IEnumerable<SachDTO>> GetAllAsync();
        Task<SachDTO> GetByMaSach(string MaSach);
        Task<SachDTO> AddAsync(SachDTO dto);
        Task<SachDTO> UpdateAsync(SachDTO dto, string MaSach);
        Task<bool> DeleteAsync(string MaSach);
        Task<PagedResult<SachDTO>> GetPageAsync(
              int page,
              int pageSize,
              string? search
          );
    }
}
