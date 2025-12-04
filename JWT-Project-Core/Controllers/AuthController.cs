using JWT_Project_Core.DTO;
using JWT_Project_Core.Interface;
using JWT_Project_Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace JWT_Project_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var result = await _authService.LoginAsync(login);
            if (result == null)
                return Unauthorized("Username or password incorrect");

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] LogoutDto dto)
        {
            var newToken = await _authService.RefreshTokenAsync(dto.RefreshToken);
            if (newToken == null) return Unauthorized("Invalid refresh token");

            return Ok(new { token = newToken });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO newUser)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value!.Errors.Any())
                    .ToDictionary(
                        k => k.Key,
                        v => v.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(errors);
            }

            var result = await _authService.RegisterAsync(newUser);
            if (result == "Username already exists")
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
        {
            var result = await _authService.RemoveRefreshTokenAsync(dto.RefreshToken);
            if (!result) return BadRequest("Không tìm thấy refresh token");
            return Ok(new { message = "Đăng xuất thành công" });
        }


    }
}
