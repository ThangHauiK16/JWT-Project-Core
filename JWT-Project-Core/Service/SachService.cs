using AutoMapper;
using JWT_Project_Core.Data;
using JWT_Project_Core.DTO;
using JWT_Project_Core.Interface;
using JWT_Project_Core.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace JWT_Project_Core.Service
{
    public class SachService  : ISachService
    {
        public readonly ApplicationDbContext context;
        public readonly IMapper mapper;
        public readonly IFileService fileService;
        public SachService(ApplicationDbContext context, IMapper mapper , IFileService fileService)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileService = fileService;
        }
        public async Task<IEnumerable<SachDTO>> GetAllAsync() 
        {
            try
            {
                var Sach = await context.Saches.ToListAsync();
                Log.Information("Lay danh sach san pham thang cong!");
                return mapper.Map<IEnumerable<SachDTO>>(Sach);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetAllAsync: unexpected error occurred while retrieving Sach");
                throw;
            }
        }
        public async Task<PagedResult<SachDTO>> GetPageAsync(    int page, int pageSize,string? search  )
                                
        {
            try
            {
                var query = context.Saches.AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();

                    query = query.Where(s =>
                        s.TenSach!.ToLower().Contains(search) ||
                        s.MaSach!.ToLower().Contains(search) ||
                        s.TheLoai!.ToLower().Contains(search)
                    );
                }

                var totalItems = await query.CountAsync();

                var items = await query
                    .OrderBy(s => s.MaSach)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<SachDTO>(
                    mapper.Map<IEnumerable<SachDTO>>(items),
                    totalItems,
                    page,
                    pageSize
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetPageAsync: Error when paging Sach");
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await context.Saches
                    .Select(s => s.TheLoai)
                    .Distinct()
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetAllCategoriesAsync: unexpected error occurred");
                throw;
            }
        }
    
        public async Task<SachDTO> GetByMaSach(string MaSach)
        {
            try
            {
                var sach = await context.Saches.FindAsync(MaSach);
                if(sach == null)
                {
                    Log.Warning("Khong tim thay sach co ma {MaSach} nay !" , MaSach);
                    return null!;
                }
                else
                {
                    Log.Information("Lay sach {TenSach} theo ma  {MaSach} thanh cong !", sach.TenSach, sach.MaSach);
                    return mapper.Map<SachDTO>(sach);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetByMaSach : Unexpected error while retrieving student with maSach {MaSach}", MaSach);
                throw;
            }
       }
        public async Task<SachDTO> AddAsync(SachDTO dto)
        {
            try
            {
                var exit = await context.Saches.AnyAsync(s => s.MaSach == dto.MaSach);
                if (exit)
                {
                    Log.Warning("AddAsync: Sach with name {TenSach} already exists", dto.TenSach);
                    throw new InvalidOperationException($"Sach with name {dto.TenSach} already exists");
                }
                if (dto.ImageFile != null)
                {
                    dto.ImageUrl = await fileService.SaveImageAsync(dto.ImageFile);
                }
                var sach = mapper.Map<Sach>(dto);
                context.Saches.Add(sach);
                await context.SaveChangesAsync();
                Log.Information("Them sach thanh cong !");
                return mapper.Map<SachDTO>(sach);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AddAsync: Unexpected error while adding sach {Name}", dto.TenSach);
                throw;
            }
        }
        public async Task<SachDTO> UpdateAsync(SachDTO dto , string MaSach)
        {
            try
            {
                var sach = await context.Saches.FindAsync(MaSach);
                if(sach == null)
                {
                    Log.Warning("UpdateAsync: Khong Tim thay sach co ma bang {MaSach}", MaSach);
                    return null!;
                }
                if (dto.ImageFile != null)
                {
                    if (!string.IsNullOrEmpty(sach.ImageUrl))
                        fileService.DeleteImage(sach.ImageUrl);

                    dto.ImageUrl = await fileService.SaveImageAsync(dto.ImageFile);
                }
                else
                {
                    dto.ImageUrl = sach.ImageUrl;
                }

                mapper.Map(dto , sach);
                await context.SaveChangesAsync();
                Log.Information("UpdateAsync: Updated student with ma {MaSach}", MaSach);
                return mapper.Map<SachDTO>(sach);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UpdateAsync: Unexpected error while updating book {MaSach}", dto.MaSach);
                throw;
            }
        }
        public async Task<bool> DeleteAsync(string MaSach)
        {
            try
            {
                var exit = await context.Saches.FindAsync(MaSach);
                if(exit == null)
                {
                    Log.Warning("DeleteAsync: Book with ma {MaSach} has not exists", MaSach);
                    return false;
                }
                context.Saches.Remove(exit);
                await context.SaveChangesAsync();
                Log.Information("DeleteAsync: Deleted student with Id {MaSach}", MaSach);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "DeleteAsync: Unexpected error while Deleting book {MaSach}", MaSach);
                throw;
            }
        }

    }
}
