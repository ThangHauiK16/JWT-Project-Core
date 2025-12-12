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
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OrderService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<OrderDTO>> GetPagedAsync(
                 int page,
                 int pageSize,
                 string? search
             )
        {
            try
            {
                var query = _context.Orders
                    .Include(h => h.OrderBooks).ThenInclude(hs => hs.Sach)
                    .Include(h => h.User)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                   
                    if (Guid.TryParse(search, out Guid guidSearch))
                    {
                        query = query.Where(x => x.MaHoaDon == guidSearch);
                    }
                   
                    else if (DateTime.TryParse(search, out DateTime dateSearch))
                    {
                        query = query.Where(x => x.NgayTao.Date == dateSearch.Date);
                    }
                   
                    else
                    {
                        query = query.Where(x => x.Username!.Contains(search));
                    }
                }

                var totalItems = await query.CountAsync();

                var items = await query
                    .OrderByDescending(x => x.NgayTao)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<OrderDTO>(
                    _mapper.Map<IEnumerable<OrderDTO>>(items),
                    totalItems,
                    page,
                    pageSize
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetPagedAsync: unexpected error");
                throw;
            }
        }



        public async Task<OrderDTO> GetByIdAsync(Guid id)
        {
            try
            {
                var hd = await _context.Orders
                    .Include(h => h.OrderBooks)
                    .ThenInclude(hs => hs.Sach)
                    .Include(h => h.User)
                    .FirstOrDefaultAsync(h => h.MaHoaDon == id);

                if (hd == null)
                {
                    Log.Warning("Không tìm thấy hóa đơn {MaHoaDon}", id);
                    return null!;
                }

                return _mapper.Map<OrderDTO>(hd);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetByIdAsync: Unexpected error while retrieving HoaDon {MaHoaDon}", id);
                throw;
            }
        }
        public async Task<List<OrderDTO>> GetByUserAsync(string username)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(h => h.Username == username)
                    .Include(h => h.OrderBooks)
                        .ThenInclude(hs => hs.Sach)
                    .Include(h => h.User)
                    .OrderByDescending(h => h.NgayTao)
                    .ToListAsync();

                return _mapper.Map<List<OrderDTO>>(orders);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetByUserAsync: unexpected error for user {Username}", username);
                throw;
            }
        }


        public async Task<OrderDTO> AddAsync(OrderDTO dto)
        {
            try
            {
                
                foreach (var item in dto.Order_Books)
                {
                    var sach = await _context.Books.FirstOrDefaultAsync(x => x.MaSach == item.MaSach);

                    if (sach == null)
                        throw new InvalidOperationException($"Sách {item.MaSach} không tồn tại");

                  
                    item.TenSach = sach.TenSach;
                    item.TacGia = sach.TenTacGia;
                    item.Gia = sach.GiaBan;
                    item.HinhAnh = sach.ImageUrl;
                }

                var hoaDon = _mapper.Map<Order>(dto);
                hoaDon.Username = dto.Username;
                hoaDon.TrangThai = EnumStatus.pending;

                foreach (var hs in hoaDon.OrderBooks)
                {
                    hs.HoaDon = hoaDon;
                }

                _context.Orders.Add(hoaDon);
                await _context.SaveChangesAsync();

                return _mapper.Map<OrderDTO>(hoaDon);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AddAsync: Unexpected error");
                throw;
            }
        }


        public async Task<OrderDTO> UpdateAsync(OrderDTO dto, Guid id)
        {
            try
            {
                var hoaDon = await _context.Orders
                    .Include(h => h.OrderBooks)
                    .FirstOrDefaultAsync(h => h.MaHoaDon == id);

                if (hoaDon == null)
                {
                    Log.Warning("UpdateAsync: Không tìm thấy hóa đơn {MaHoaDon}", id);
                    return null!;
                }

                _context.OrderBooks.RemoveRange(hoaDon.OrderBooks);
                hoaDon.OrderBooks = dto.Order_Books.Select(hs => new OrderBook
                {
                    MaHoaDon = id,
                    MaSach = hs.MaSach,
                    SoLuong = hs.SoLuong
                }).ToList();

                await _context.SaveChangesAsync();

                Log.Information("Cập nhật hóa đơn {MaHoaDon} thành công", id);
                return _mapper.Map<OrderDTO>(hoaDon);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdateAsync: Unexpected error while updating HoaDon {MaHoaDon}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var hoaDon = await _context.Orders
                    .Include(h => h.OrderBooks)
                    .FirstOrDefaultAsync(h => h.MaHoaDon == id);

                if (hoaDon == null)
                {
                    Log.Warning("DeleteAsync: Hóa đơn {MaHoaDon} không tồn tại", id);
                    return false;
                }

                _context.OrderBooks.RemoveRange(hoaDon.OrderBooks);
                _context.Orders.Remove(hoaDon);
                await _context.SaveChangesAsync();

                Log.Information("Xóa hóa đơn {MaHoaDon} thành công", id);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeleteAsync: Unexpected error while deleting HoaDon {MaHoaDon}", id);
                throw;
            }
        }
        public async Task<bool> ApproveAsync(Guid id)
        {
            try
            {
                var hoaDon = await _context.Orders.FirstOrDefaultAsync(h => h.MaHoaDon == id);

                if (hoaDon == null)
                {
                    Log.Warning("ApproveAsync: Không tìm thấy hóa đơn {MaHoaDon}", id);
                    return false;
                }

                hoaDon.TrangThai = EnumStatus.success;
                await _context.SaveChangesAsync();

                Log.Information("ApproveAsync: Duyệt hóa đơn {MaHoaDon} thành công", id);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ApproveAsync: Unexpected error while approving HoaDon {MaHoaDon}", id);
                throw;
            }
        }


       
        public async Task<bool> CancelAsync(Guid id)
        {
            try
            {
                var hoaDon = await _context.Orders
                    .Include(h => h.OrderBooks)
                    .FirstOrDefaultAsync(h => h.MaHoaDon == id);

                if (hoaDon == null)
                {
                    Log.Warning("CancelAsync: Không tìm thấy hóa đơn {MaHoaDon}", id);
                    return false;
                }
                if (hoaDon.TrangThai == EnumStatus.cancel)
                {
                    Log.Warning("CancelAsync: Hóa đơn {MaHoaDon} đã ở trạng thái cancel", id);
                    return false;
                }

                
                foreach (var item in hoaDon.OrderBooks)
                {
                    var sach = await _context.Books.FirstOrDefaultAsync(x => x.MaSach == item.MaSach);
                    if (sach != null)
                    {
                        sach.SoLuong += item.SoLuong; 
                    }
                }
                hoaDon.TrangThai = EnumStatus.cancel;

                await _context.SaveChangesAsync();

                Log.Information("CancelAsync: Hủy hóa đơn {MaHoaDon} và hoàn tồn kho thành công", id);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "CancelAsync: Unexpected error while cancelling HoaDon {MaHoaDon}", id);
                throw;
            }
        }


    }
}
