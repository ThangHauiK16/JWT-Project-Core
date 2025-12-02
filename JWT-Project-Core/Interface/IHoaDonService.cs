using JWT_Project_Core.DTO;

namespace JWT_Project_Core.Interface
{
    public interface IHoaDonService
    {
       Task<PagedResult<HoaDonDTO>> GetPagedAsync(
                 int page,
                 int pageSize,
                 string? search
             );
        Task<HoaDonDTO> GetByIdAsync(Guid id);
        Task<List<HoaDonDTO>> GetByUserAsync(string username);
        Task<HoaDonDTO> AddAsync(HoaDonDTO dto);
        Task<HoaDonDTO> UpdateAsync(HoaDonDTO dto, Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ApproveAsync(Guid id);
        Task<bool> CancelAsync(Guid id);


    }
}
