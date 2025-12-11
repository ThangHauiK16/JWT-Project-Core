using JWT_Project_Core.Interface;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Project_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;

        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        [HttpGet("metrics")]
        public async Task<IActionResult> GetMetrics()
        {
            var data = await _service.GetDashboardDataAsync();
            return Ok(data);
        }
    }
}
