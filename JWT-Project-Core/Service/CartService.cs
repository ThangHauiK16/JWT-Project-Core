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

       
        public async Task<CartDTO> AddToCartAsync(string username, string maSach, int soLuong)
        {
            try
            {
                Log.Information("AddToCartAsync: Adding {SoLuong} of {MaSach} to cart {Username}", soLuong, maSach, username);

               
                var book = await _context.Books.FirstOrDefaultAsync(x => x.MaSach == maSach);
                if (book == null)
                    throw new Exception("Sách không tồn tại!");

                if (soLuong <= 0)
                    throw new Exception("Số lượng phải lớn hơn 0!");

                var cart = await _context.Carts
                    .Include(c => c.CartItems!)
                    .FirstOrDefaultAsync(c => c.Username == username);

                if (cart == null)
                {
                    cart = new Cart { Username = username, CartItems = new List<CartItem>() };
                    _context.Carts.Add(cart);
                }

                cart.CartItems ??= new List<CartItem>();

                var item = cart.CartItems.FirstOrDefault(x => x.MaSach == maSach);

                int currentQty = item?.SoLuong ?? 0;

                if (currentQty + soLuong > book.SoLuong)
                {
                    throw new Exception($"Chỉ còn {book.SoLuong} sản phẩm trong kho!");
                }

                if (item == null)
                {
                    cart.CartItems.Add(new CartItem
                    {
                        MaSach = maSach,
                        SoLuong = soLuong
                    });
                }
                else
                {
                    item.SoLuong += soLuong;
                }

                await _context.SaveChangesAsync();

                return _mapper.Map<CartDTO>(cart);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AddToCartAsync Error");
                throw;
            }
        }

       
        public async Task<CartDTO> UpdateQuantityAsync(string username, string maSach, int soLuong)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems!)
                    .FirstOrDefaultAsync(c => c.Username == username);

                if (cart == null) return null!;

                var item = cart.CartItems!.FirstOrDefault(x => x.MaSach == maSach);
                if (item == null) return null!;

                var book = await _context.Books.FirstOrDefaultAsync(x => x.MaSach == maSach);
                if (book == null) throw new Exception("Sách không tồn tại!");

                if (soLuong > book.SoLuong)
                    throw new Exception($"Tồn kho chỉ còn {book.SoLuong}!");

                item.SoLuong = soLuong;
                await _context.SaveChangesAsync();

                return _mapper.Map<CartDTO>(cart);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdateQuantityAsync Error");
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
       
        public async Task<CartDTO> IncreaseQuantityAsync(string username, string maSach)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems!)
                    .FirstOrDefaultAsync(c => c.Username == username);

                var item = cart?.CartItems?.FirstOrDefault(x => x.MaSach == maSach);
                if (item == null) return null!;

                var book = await _context.Books.FirstOrDefaultAsync(x => x.MaSach == maSach);
                if (book == null) throw new Exception("Sách không tồn tại!");

                if (item.SoLuong + 1 > book.SoLuong)
                    throw new Exception($"Không thể tăng thêm. Tồn kho chỉ còn {book.SoLuong}!");

                item.SoLuong++;
                await _context.SaveChangesAsync();

                return _mapper.Map<CartDTO>(cart);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "IncreaseQuantityAsync Error");
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


        

        public async Task<OrderDTO> CheckoutAsync(string username)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.CartItems!)
                    .FirstOrDefaultAsync(c => c.Username == username);

                if (cart == null || !cart.CartItems!.Any())
                    throw new Exception("Giỏ hàng trống!");

               
                foreach (var ci in cart.CartItems!)
                {
                    var book = await _context.Books.FirstOrDefaultAsync(b => b.MaSach == ci.MaSach);

                    if (book == null)
                        throw new Exception($"Sách {ci.MaSach} không tồn tại!");

                    if (ci.SoLuong > book.SoLuong)
                        throw new Exception($"Sản phẩm {book.TenSach} chỉ còn {book.SoLuong} trong kho!");
                }

                
                foreach (var ci in cart.CartItems)
                {
                    var book = await _context.Books.FirstAsync(b => b.MaSach == ci.MaSach);
                    book.SoLuong -= ci.SoLuong;
                }

               
                var order = new Order
                {
                    Username = username,
                    NgayTao = DateTime.UtcNow,
                    TrangThai = EnumStatus.pending,
                    Order_Books = cart.CartItems.Select(ci => new Order_Book
                    {
                        MaSach = ci.MaSach,
                        SoLuong = ci.SoLuong
                    }).ToList()
                };

                _context.Orders.Add(order);

              
                _context.CartItems.RemoveRange(cart.CartItems);

                await _context.SaveChangesAsync();

                return _mapper.Map<OrderDTO>(order);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CheckoutAsync Error");
                throw;
            }
        }

    }
}
