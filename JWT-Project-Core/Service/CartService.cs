using AutoMapper;
using JWT_Project_Core.Data;
using JWT_Project_Core.DTO;
using JWT_Project_Core.Enum;
using JWT_Project_Core.Interface;
using JWT_Project_Core.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace JWT_Project_Core.Service
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CartService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Lấy giỏ hàng
        public async Task<CartDTO> GetCartAsync(string username)
        {
            try
            {
                Log.Information("GetCartAsync: Getting cart for user {Username}", username);

                var cart = await _context.Carts
                    .Include(c => c.CartItems!)
                    .ThenInclude(i => i.Sach)
                    .FirstOrDefaultAsync(c => c.Username == username);

                if (cart == null)
                {
                    Log.Warning("GetCartAsync: Cart not found for {Username}, creating new one", username);

                    cart = new Cart
                    {
                        Username = username,
                        CartItems = new List<CartItem>()
                    };

                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                Log.Information("GetCartAsync: Retrived cart for user {Username} with {Count} items", username, cart.CartItems!.Count);

                return _mapper.Map<CartDTO>(cart);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetCartAsync: Unexpected error getting cart for {Username}", username);
                throw;
            }
        }

        // Thêm sản phẩm vào giỏ
        public async Task<CartDTO> AddToCartAsync(string username, string maSach, int soLuong)
        {
            try
            {
                Log.Information("AddToCartAsync: Adding {SoLuong} of {MaSach} to cart {Username}", soLuong, maSach, username);

                var cart = await _context.Carts
                    .Include(c => c.CartItems!)
                    .FirstOrDefaultAsync(c => c.Username == username);

                if (cart == null)
                {
                    Log.Warning("AddToCartAsync: Cart not found for {Username}, creating new one", username);

                    cart = new Cart
                    {
                        Username = username,
                        CartItems = new List<CartItem>()
                    };

                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                }

                cart.CartItems ??= new List<CartItem>();

                var item = cart.CartItems.FirstOrDefault(x => x.MaSach == maSach);

                if (item == null)
                {
                    Log.Information("AddToCartAsync: Item {MaSach} not found, adding new", maSach);

                    cart.CartItems.Add(new CartItem
                    {
                        MaSach = maSach,
                        SoLuong = soLuong
                    });
                }
                else
                {
                    Log.Information("AddToCartAsync: Item {MaSach} exists, increasing quantity to {NewQty}", maSach, item.SoLuong + soLuong);
                    item.SoLuong += soLuong;
                }

                await _context.SaveChangesAsync();

                Log.Information("AddToCartAsync: Updated cart for {Username}", username);

                return _mapper.Map<CartDTO>(cart);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AddToCartAsync: Unexpected error for user {Username}", username);
                throw;
            }
        }

        // Update số lượng
        public async Task<CartDTO> UpdateQuantityAsync(string username, string maSach, int soLuong)
        {
            try
            {
                Log.Information("UpdateQuantityAsync: Updating {MaSach} to quantity {SoLuong} for {Username}", maSach, soLuong, username);

                var cart = await _context.Carts
                    .Include(c => c.CartItems!)
                    .FirstOrDefaultAsync(c => c.Username == username);

                if (cart == null || cart.CartItems == null)
                {
                    Log.Warning("UpdateQuantityAsync: Cart or cart items not found for {Username}", username);
                    return null!;
                }

                var item = cart.CartItems.FirstOrDefault(x => x.MaSach == maSach);
                if (item == null)
                {
                    Log.Warning("UpdateQuantityAsync: Item {MaSach} not found for {Username}", maSach, username);
                    return null!;
                }

                item.SoLuong = soLuong;
                await _context.SaveChangesAsync();

                Log.Information("UpdateQuantityAsync: Updated quantity for {MaSach} successfully", maSach);

                return _mapper.Map<CartDTO>(cart);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdateQuantityAsync: Unexpected error for user {Username}", username);
                throw;
            }
        }

        // Xóa 1 sản phẩm
        public async Task<bool> RemoveItemAsync(string username, string maSach)
        {
            try
            {
                Log.Information("RemoveItemAsync: Removing item {MaSach} from cart of {Username}", maSach, username);

                var cart = await _context.Carts
                    .Include(c => c.CartItems!)
                    .FirstOrDefaultAsync(c => c.Username == username);

                if (cart == null || cart.CartItems == null)
                {
                    Log.Warning("RemoveItemAsync: Cart not found for {Username}", username);
                    return false;
                }

                var item = cart.CartItems.FirstOrDefault(x => x.MaSach == maSach);
                if (item == null)
                {
                    Log.Warning("RemoveItemAsync: Item {MaSach} not found", maSach);
                    return false;
                }

                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();

                Log.Information("RemoveItemAsync: Removed item {MaSach} successfully", maSach);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "RemoveItemAsync: Unexpected error for user {Username}", username);
                throw;
            }
        }
        // Tăng số lượng 1 đơn vị
        public async Task<CartDTO> IncreaseQuantityAsync(string username, string maSach)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems!)
                    .FirstOrDefaultAsync(c => c.Username == username);

                if (cart == null || cart.CartItems == null)
                    return null!;

                var item = cart.CartItems.FirstOrDefault(x => x.MaSach == maSach);
                if (item == null)
                    return null!;

                item.SoLuong += 1;
                await _context.SaveChangesAsync();

                return _mapper.Map<CartDTO>(cart);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "IncreaseQuantityAsync: Unexpected error for user {Username}", username);
                throw;
            }
        }

        // Giảm số lượng 1 đơn vị
        public async Task<CartDTO> DecreaseQuantityAsync(string username, string maSach)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems!)
                    .FirstOrDefaultAsync(c => c.Username == username);

                if (cart == null || cart.CartItems == null)
                    return null!;

                var item = cart.CartItems.FirstOrDefault(x => x.MaSach == maSach);
                if (item == null)
                    return null!;

                if (item.SoLuong > 1)
                {
                    item.SoLuong -= 1;
                }
                else
                {
                    // Nếu số lượng = 1, giảm nữa => xóa sản phẩm khỏi giỏ
                    _context.CartItems.Remove(item);
                }

                await _context.SaveChangesAsync();

                return _mapper.Map<CartDTO>(cart);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DecreaseQuantityAsync: Unexpected error for user {Username}", username);
                throw;
            }
        }


        // Thanh toán
        public async Task<HoaDonDTO> CheckoutAsync(string username)
        {
            try
            {
                Log.Information("CheckoutAsync: Processing checkout for {Username}", username);

                var cart = await _context.Carts
                    .Include(c => c.CartItems!)
                    .FirstOrDefaultAsync(c => c.Username == username);

                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                {
                    Log.Warning("CheckoutAsync: Cart is empty for {Username}", username);
                    throw new InvalidOperationException("Giỏ hàng trống!");
                }

                var hoaDon = new HoaDon
                {
                    Username = username,
                    NgayTao = DateTime.UtcNow,
                    TrangThai = EnumStatus.pending,
                    HoaDon_Saches = cart.CartItems.Select(ci => new HoaDon_Sach
                    {
                        MaSach = ci.MaSach,
                        SoLuong = ci.SoLuong
                    }).ToList()
                };

                _context.HoaDons.Add(hoaDon);

                Log.Information("CheckoutAsync: Created invoice for {Username}, deleting cart items", username);

                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();

                Log.Information("CheckoutAsync: Checkout completed successfully for {Username}", username);

                return _mapper.Map<HoaDonDTO>(hoaDon);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CheckoutAsync: Unexpected error during checkout for {Username}", username);
                throw;
            }
        }
    }
}
