using JWT_Project_Core.DTO;

namespace JWT_Project_Core.Interface
{
    public interface IBookService
    {
        Task<IEnumerable<BookDTO>> GetAllAsync();
        Task<BookDTO> GetByMaSach(string MaSach);
        Task<BookDTO> AddAsync(BookDTO dto);
        Task<BookDTO> UpdateAsync(BookDTO dto, string MaSach);
        Task<bool> DeleteAsync(string MaSach);
        Task<PagedResult<BookDTO>> GetPageAsync(
              int page,
              int pageSize,
              string? search
          );
        Task<IEnumerable<string>> GetAllCategoriesAsync();
        Task<PagedResult<BookDTO>> GetPageSortByPriceAsync(int page, int pageSize, string? search, string? category, string? sortPrice);
        Task<int> ImportExcelAsync(IFormFile file);


    }
}
