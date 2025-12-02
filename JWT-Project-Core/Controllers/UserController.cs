using JWT_Project_Core.DTO;
using JWT_Project_Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Project_Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(int page = 1, int pageSize = 10, string? keyword = null)
        {
            return Ok(await _userService.GetUsersAsync(page, pageSize, keyword));
        }


        [HttpGet("{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await _userService.GetUserAsync(username);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpPut("{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(string username, [FromBody] UserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.UpdateUserAsync(username, dto);
            return success ? Ok() : NotFound();
        }

        [HttpDelete("{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            var success = await _userService.DeleteUserAsync(username);
            return success ? Ok() : NotFound();
        }
    }

}
