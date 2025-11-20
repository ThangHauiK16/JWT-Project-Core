using AutoMapper;
using JWT_Project_Core.Data;
using JWT_Project_Core.DTO;
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

        public async Task<IEnumerable<HoaDonDTO>> GetAllAsync()
        {
            try
            {
                var list = await _context.HoaDons
                    .Include(h => h.HoaDon_Saches)
                    .ThenInclude(hs => hs.Sach)
                    .Include(h => h.User)
                    .ToListAsync();

                Log.Information("Lấy danh sách hóa đơn thành công!");
                return _mapper.Map<IEnumerable<HoaDonDTO>>(list);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetAllAsync: Unexpected error while retrieving HoaDon");
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

        public async Task<HoaDonDTO> AddAsync(HoaDonDTO dto)
        {
            try
            {
                var hoaDon = _mapper.Map<HoaDon>(dto);

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

    }
}
