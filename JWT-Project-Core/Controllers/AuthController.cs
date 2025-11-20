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
            var token = await _authService.LoginAsync(login);
            if (token == null)
                return Unauthorized("Username or password incorrect");

            return Ok(new { token , login.Username});
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO newUser)
        {
            if (!ModelState.IsValid)
            {
                Log.Warning("Register student failed: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterAsync(newUser);
            if (result == "Username already exists")
                return BadRequest(result);

            return Ok(result);
        }
    }
}
