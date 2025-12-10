using JWT_Project_Core.DTO;
using JWT_Project_Core.Interface;
using JWT_Project_Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace JWT_Project_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        public readonly IBookService bookService;
        public BookController(IBookService bookService)
        {
            this.bookService = bookService;
        }
        [HttpGet]
        public async Task<IActionResult> GetSachs()
        {
            var data = await bookService.GetAllAsync();
            return Ok(data);
        }
        [HttpGet("page")]
        public async Task<IActionResult> GetPage(
            int page = 1,
            int pageSize = 10,
            string? search = null
            )
        {
            var result = await bookService.GetPageAsync(page, pageSize, search);
            return Ok(result);
        }
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await bookService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{MaSach}")]
        public async Task<IActionResult> GetSachByMaSach(string MaSach)
        {
            var data = await bookService.GetByMaSach(MaSach);
            if (data == null) return NotFound();
            return Ok(data);
        }

        [HttpGet("sort")]
        public async Task<IActionResult> GetSortByPrice(
                                                        int page = 1,
                                                        int pageSize = 10,
                                                        string? search = "",
                                                        string? category = "",
                                                        string? sortPrice = "all")
        {
            var result = await bookService.GetPageSortByPriceAsync(page, pageSize, search, category, sortPrice);
            return Ok(result);
        }


        [HttpPost]
       
        public async Task<IActionResult> AddSach([FromForm] BookDTO data)
        {
            if (!ModelState.IsValid)
            {
                Log.Warning("Add book failed: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                return BadRequest();
            }
            var newSach = await bookService.AddAsync(data);
            return Ok(newSach);
        }
        [HttpPut("{MaSach}")]
       
        public async Task<IActionResult> UpdateSach([FromForm] BookDTO data, string MaSach)
        {
            if (!ModelState.IsValid)
            {
                Log.Warning("Upadate book failed: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                return BadRequest();
            }
            var updated = await bookService.UpdateAsync(data, MaSach);
            if(updated == null) return NotFound();  
            return Ok(updated);
        }
        [HttpDelete("{MaSach}")]
        
        public async Task<IActionResult> DeleteSach(string MaSach)
        {
            var Delete = await bookService.DeleteAsync(MaSach);
            if (!Delete) return NotFound();
            return Ok();
        }
    }
}
