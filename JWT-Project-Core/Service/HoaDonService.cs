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
    public class HoaDonService : IHoaDonService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public HoaDonService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<HoaDonDTO>> GetPagedAsync(
                 int page,
                 int pageSize,
                 string? search
             )
        {
            try
            {
                var query = _context.HoaDons
                    .Include(h => h.HoaDon_Saches).ThenInclude(hs => hs.Sach)
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

                return new PagedResult<HoaDonDTO>(
                    _mapper.Map<IEnumerable<HoaDonDTO>>(items),
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



        public async Task<HoaDonDTO> GetByIdAsync(Guid id)
        {
            try
            {
                var hd = await _context.HoaDons
                    .Include(h => h.HoaDon_Saches)
                    .ThenInclude(hs => hs.Sach)
                    .Include(h => h.User)
                    .FirstOrDefaultAsync(h => h.MaHoaDon == id);

                if (hd == null)
                {
                    Log.Warning("Không tìm thấy hóa đơn {MaHoaDon}", id);
                    return null!;
                }

                return _mapper.Map<HoaDonDTO>(hd);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetByIdAsync: Unexpected error while retrieving HoaDon {MaHoaDon}", id);
                throw;
            }
        }
        public async Task<List<HoaDonDTO>> GetByUserAsync(string username)
        {
            try
            {
                var orders = await _context.HoaDons
                    .Where(h => h.Username == username)
                    .Include(h => h.HoaDon_Saches)
                        .ThenInclude(hs => hs.Sach)
                    .Include(h => h.User)
                    .OrderByDescending(h => h.NgayTao)
                    .ToListAsync();

                return _mapper.Map<List<HoaDonDTO>>(orders);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetByUserAsync: unexpected error for user {Username}", username);
                throw;
            }
        }


        public async Task<HoaDonDTO> AddAsync(HoaDonDTO dto)
        {
            try
            {
                var hoaDon = _mapper.Map<HoaDon>(dto);

                hoaDon.TrangThai = EnumStatus.pending;

                hoaDon.Username = dto.Username;
                foreach (var hs in hoaDon.HoaDon_Saches)
                {
                    var sachExist = await _context.Saches.FindAsync(hs.MaSach);
                    if (sachExist == null)
                    {
                        Log.Warning("AddAsync: Sach {MaSach} không tồn tại", hs.MaSach);
                        throw new InvalidOperationException($"Sach {hs.MaSach} không tồn tại");
                    }
                    hs.HoaDon = hoaDon; 
                }

                _context.HoaDons.Add(hoaDon);
                await _context.SaveChangesAsync();

                Log.Information("Thêm hóa đơn {MaHoaDon} thành công!", hoaDon.MaHoaDon);
                return _mapper.Map<HoaDonDTO>(hoaDon);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AddAsync: Unexpected error while adding HoaDon");
                throw;
            }
        }

        public async Task<HoaDonDTO> UpdateAsync(HoaDonDTO dto, Guid id)
        {
            try
            {
                var hoaDon = await _context.HoaDons
                    .Include(h => h.HoaDon_Saches)
                    .FirstOrDefaultAsync(h => h.MaHoaDon == id);

                if (hoaDon == null)
                {
                    Log.Warning("UpdateAsync: Không tìm thấy hóa đơn {MaHoaDon}", id);
                    return null!;
                }

                _context.HoaDon_Saches.RemoveRange(hoaDon.HoaDon_Saches);
                hoaDon.HoaDon_Saches = dto.HoaDon_Saches.Select(hs => new HoaDon_Sach
                {
                    MaHoaDon = id,
                    MaSach = hs.MaSach,
                    SoLuong = hs.SoLuong
                }).ToList();

                await _context.SaveChangesAsync();

                Log.Information("Cập nhật hóa đơn {MaHoaDon} thành công", id);
                return _mapper.Map<HoaDonDTO>(hoaDon);
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
                var hoaDon = await _context.HoaDons
                    .Include(h => h.HoaDon_Saches)
                    .FirstOrDefaultAsync(h => h.MaHoaDon == id);

                if (hoaDon == null)
                {
                    Log.Warning("DeleteAsync: Hóa đơn {MaHoaDon} không tồn tại", id);
                    return false;
                }

                _context.HoaDon_Saches.RemoveRange(hoaDon.HoaDon_Saches);
                _context.HoaDons.Remove(hoaDon);
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
                var hoaDon = await _context.HoaDons.FirstOrDefaultAsync(h => h.MaHoaDon == id);

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
                var hoaDon = await _context.HoaDons.FirstOrDefaultAsync(h => h.MaHoaDon == id);

                if (hoaDon == null)
                {
                    Log.Warning("CancelAsync: Không tìm thấy hóa đơn {MaHoaDon}", id);
                    return false;
                }

                hoaDon.TrangThai = EnumStatus.cancel;
                await _context.SaveChangesAsync();

                Log.Information("CancelAsync: Hủy duyệt hóa đơn {MaHoaDon} thành công", id);
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
