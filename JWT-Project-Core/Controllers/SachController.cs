using JWT_Project_Core.DTO;
using JWT_Project_Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace JWT_Project_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SachController : ControllerBase
    {
        public readonly ISachService sachService;
        public SachController(ISachService sachService)
        {
            this.sachService = sachService;
        }
        [HttpGet]
        public async Task<IActionResult> GetSachs()
        {
            var data = await sachService.GetAllAsync();
            return Ok(data);
        }
        [HttpGet("page")]
        public async Task<IActionResult> GetPage(
            int page = 1,
            int pageSize = 10,
            string? search = null
            )
        {
            var result = await sachService.GetPageAsync(page, pageSize, search);
            return Ok(result);
        }
        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await sachService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{MaSach}")]
        public async Task<IActionResult> GetSachByMaSach(string MaSach)
        {
            var data = await sachService.GetByMaSach(MaSach);
            if (data == null) return NotFound();
            return Ok(data);
        }
        [HttpPost]
       
        public async Task<IActionResult> AddSach([FromForm] SachDTO data)
        {
            if (!ModelState.IsValid)
            {
                Log.Warning("Add book failed: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                return BadRequest();
            }
            var newSach = await sachService.AddAsync(data);
            return Ok(newSach);
        }
        [HttpPut("{MaSach}")]
       
        public async Task<IActionResult> UpdateSach([FromForm] SachDTO data, string MaSach)
        {
            if (!ModelState.IsValid)
            {
                Log.Warning("Upadate book failed: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                return BadRequest();
            }
            var updated = await sachService.UpdateAsync(data, MaSach);
            if(updated == null) return NotFound();  
            return Ok(updated);
        }
        [HttpDelete("{MaSach}")]
        
        public async Task<IActionResult> DeleteSach(string MaSach)
        {
            var Delete = await sachService.DeleteAsync(MaSach);
            if (!Delete) return NotFound();
            return Ok();
        }
    }
}
