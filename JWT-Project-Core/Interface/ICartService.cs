using JWT_Project_Core.DTO;

namespace JWT_Project_Core.Interface
{
    public interface ICartService
    {
        Task<CartDTO> GetCartAsync(string username);
        Task<CartDTO> AddToCartAsync(string username, string maSach, int soLuong);
        Task<CartDTO> UpdateQuantityAsync(string username, string maSach, int soLuong);
        Task<bool> RemoveItemAsync(string username, string maSach);
        Task<CartDTO> IncreaseQuantityAsync(string username, string maSach);
        Task<CartDTO> DecreaseQuantityAsync(string username, string maSach);

        Task<HoaDonDTO> CheckoutAsync(string username);
    }
}
