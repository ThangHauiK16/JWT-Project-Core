using JWT_Project_Core.DTO;

namespace JWT_Project_Core.Interface
{
    public interface IOrderService
    {
       Task<PagedResult<OrderDTO>> GetPagedAsync(
                 int page,
                 int pageSize,
                 string? search
             );
        Task<OrderDTO> GetByIdAsync(Guid id);
        Task<List<OrderDTO>> GetByUserAsync(string username);
        Task<OrderDTO> AddAsync(OrderDTO dto);
        Task<OrderDTO> UpdateAsync(OrderDTO dto, Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ApproveAsync(Guid id);
        Task<bool> CancelAsync(Guid id);


    }
}
