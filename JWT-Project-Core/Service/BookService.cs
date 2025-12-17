using AutoMapper;
using ClosedXML.Excel;
using JWT_Project_Core.Data;
using JWT_Project_Core.DTO;
using JWT_Project_Core.Interface;
using JWT_Project_Core.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace JWT_Project_Core.Service
{
    public class BookService  : IBookService
    {
        public readonly ApplicationDbContext context;
        public readonly IMapper mapper;
        public readonly IFileService fileService;
        public BookService(ApplicationDbContext context, IMapper mapper , IFileService fileService)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileService = fileService;
        }
        public async Task<IEnumerable<BookDTO>> GetAllAsync() 
        {
            try
            {
                var Sach = await context.Books
                     .Where(b => !b.IsDeleted)
                     .ToListAsync();
                Log.Information("Lay danh sach san pham thang cong!");
                return mapper.Map<IEnumerable<BookDTO>>(Sach);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetAllAsync: unexpected error occurred while retrieving Sach");
                throw;
            }
        }
        public async Task<PagedResult<BookDTO>> GetPageAsync(    int page, int pageSize,string? search  )
                                
        {
            try
            {
                var query = context.Books
                    .Where(b => !b.IsDeleted)
                    .AsQueryable();

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

                return new PagedResult<BookDTO>(
                    mapper.Map<IEnumerable<BookDTO>>(items),
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
                var categories = await context.Books
                    .Where(b => !b.IsDeleted)
                    .Select(s => s.TheLoai)
                    .Distinct()
                    .ToListAsync();

                return categories!;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetAllCategoriesAsync: unexpected error occurred");
                throw;
            }
        }
    
        public async Task<BookDTO> GetByMaSach(string MaSach)
        {
            try
            {
                var sach = await context.Books
                   .Where(b => b.MaSach == MaSach && !b.IsDeleted)  
                   .FirstOrDefaultAsync();
                if (sach == null)
                {
                    Log.Warning("Khong tim thay sach co ma {MaSach} nay !" , MaSach);
                    return null!;
                }
                else
                {
                    Log.Information("Lay sach {TenSach} theo ma  {MaSach} thanh cong !", sach.TenSach, sach.MaSach);
                    return mapper.Map<BookDTO>(sach);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetByMaSach : Unexpected error while retrieving student with maSach {MaSach}", MaSach);
                throw;
            }
       }

        public async Task<PagedResult<BookDTO>> GetPageSortByPriceAsync(
                                                                        int page,
                                                                        int pageSize,
                                                                        string? search,
                                                                        string? category,
                                                                        string? sortPrice)
        {
            try
            {
                var query = context.Books
                    .Where(b => !b.IsDeleted)
                    .AsQueryable();

              
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToLower();
                    query = query.Where(s =>
                        s.TenSach!.ToLower().Contains(search) ||
                        s.MaSach!.ToLower().Contains(search) ||
                        s.TheLoai!.ToLower().Contains(search)
                    );
                }

               
                if (!string.IsNullOrWhiteSpace(category) && category != "Tất cả")
                {
                    query = query.Where(s => s.TheLoai == category);
                }

                
                switch (sortPrice)
                {
                    case "under100":
                        query = query.Where(b => b.GiaBan < 100000);
                        break;

                    case "100-200":
                        query = query.Where(b => b.GiaBan >= 100000 && b.GiaBan <= 200000);
                        break;

                    case "200-500":
                        query = query.Where(b => b.GiaBan >= 200000 && b.GiaBan <= 500000);
                        break;

                    case "above500":
                        query = query.Where(b => b.GiaBan > 500000);
                        break;

                    default:
                        break;
                }

                var totalItems = await query.CountAsync();

                var items = await query
                    .OrderBy(b => b.GiaBan)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return new PagedResult<BookDTO>(
                    mapper.Map<IEnumerable<BookDTO>>(items),
                    totalItems,
                    page,
                    pageSize
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "GetPageSortByPriceAsync: error!");
                throw;
            }
        }

        public async Task<BookDTO> AddAsync(BookDTO dto)
        {
            try
            {
                var exit = await context.Books.AnyAsync(s => s.MaSach == dto.MaSach);
                if (exit)
                {
                    Log.Warning("AddAsync: Sach with name {TenSach} already exists", dto.TenSach);
                    throw new InvalidOperationException($"Sach with name {dto.TenSach} already exists");
                }
                if (dto.ImageFile != null)
                {
                    dto.ImageUrl = await fileService.SaveImageAsync(dto.ImageFile);
                }
                var sach = mapper.Map<Book>(dto);
                sach.MaSach = dto.MaSach;

                context.Books.Add(sach);
                await context.SaveChangesAsync();
                Log.Information("Them sach thanh cong !");
                return mapper.Map<BookDTO>(sach);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AddAsync: Unexpected error while adding sach {Name}", dto.TenSach);
                throw;
            }
        }
        public async Task<BookDTO> UpdateAsync(BookDTO dto , string MaSach)
        {
            try
            {
                var sach = await context.Books.FindAsync(MaSach);
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
                return mapper.Map<BookDTO>(sach);
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
                var exit = await context.Books.FindAsync(MaSach);
                if(exit == null)
                {
                    Log.Warning("DeleteAsync: Book with ma {MaSach} has not exists", MaSach);
                    return false;
                }
                exit.IsDeleted = true;
                context.Books.Update(exit);
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

        public async Task<int> ImportExcelAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new Exception("File Excel không hợp lệ");

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheet(1);

                var rows = worksheet.RangeUsed()!.RowsUsed().Skip(1); 

                int count = 0;

                foreach (var row in rows)
                {
                    var maSach = row.Cell(1).GetString().Trim();

                    if (string.IsNullOrEmpty(maSach))
                        continue;

                    if (await context.Books.AnyAsync(b => b.MaSach == maSach))
                        continue;

                    var book = new Book
                    {
                        MaSach = maSach,
                        TenSach = row.Cell(2).GetString(),
                        TheLoai = row.Cell(3).GetString(),
                        GiaNhap = row.Cell(4).GetDouble(),
                        GiaBan = row.Cell(5).GetDouble(),
                        TenTacGia = row.Cell(6).GetString(),
                        NoiDungSach = row.Cell(7).GetString(),
                        SoLuong = (int)row.Cell(8).GetDouble(),
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Books.Add(book);
                    count++;
                }

                await context.SaveChangesAsync();

                Log.Information("Import Excel thành công {Count} sách", count);

                return count;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ImportExcelAsync: error");
                throw;
            }
        }


    }
}
