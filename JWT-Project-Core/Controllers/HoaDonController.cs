using JWT_Project_Core.DTO;
using JWT_Project_Core.Interface;
using JWT_Project_Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Project_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly IHoaDonService hoaDonService;
        public HoaDonController(IHoaDonService hoaDonService)
        {
            this.hoaDonService = hoaDonService;
        }
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
             int page = 1,
             int pageSize = 10,
             string? search = null
             )
        {
            var data = await hoaDonService.GetPagedAsync(page, pageSize, search);
            return Ok(data);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetHoaDonById(Guid id)
        {
            var data = await hoaDonService.GetByIdAsync(id);
            if (data == null) return NotFound($"Mã hóa đơn {id} không tồn tại!");
            return Ok(data);
        }
        [HttpPost]
        public async Task<IActionResult> CreateHoaDon([FromBody] HoaDonDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await hoaDonService.AddAsync(dto);
            return CreatedAtAction(nameof(GetHoaDonById), new { id = result.MaHoaDon }, result);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateHoaDon(Guid id, [FromBody] HoaDonDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await hoaDonService.UpdateAsync(dto, id);
            if (result == null)
                return NotFound($"Không tìm thấy hóa đơn {id} để cập nhật!");

            return Ok(result);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteHoaDon(Guid id)
        {
            var deleted = await hoaDonService.DeleteAsync(id);

            if (!deleted)
                return NotFound($"Không tìm thấy hóa đơn {id} để xóa!");

            return Ok($"Xóa hóa đơn {id} thành công!");
        }
    }
}
