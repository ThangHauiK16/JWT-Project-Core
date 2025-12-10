using JWT_Project_Core.DTO;
using JWT_Project_Core.Interface;
using JWT_Project_Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Project_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService hoaDonService;
        public OrderController(IOrderService hoaDonService)
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

        [HttpGet("history")]
        public async Task<IActionResult> GetUserOrderHistory()
        {
            var username = User.Identity?.Name; 
            if (string.IsNullOrEmpty(username))
                return Unauthorized();

            var orders = await hoaDonService.GetByUserAsync(username);
            return Ok(orders);
        }


        [HttpPost]
        public async Task<IActionResult> CreateHoaDon([FromBody] OrderDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await hoaDonService.AddAsync(dto);
            return CreatedAtAction(nameof(GetHoaDonById), new { id = result.MaHoaDon }, result);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateHoaDon(Guid id, [FromBody] OrderDTO dto)
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

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var result = await hoaDonService.ApproveAsync(id);
            return result ? Ok("Duyệt thành công") : NotFound();
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await hoaDonService.CancelAsync(id);
            return result ? Ok("Hủy thành công") : NotFound();
        }

    }
}
