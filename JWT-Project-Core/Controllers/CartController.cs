using JWT_Project_Core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JWT_Project_Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService cartService;

        public CartController(ICartService cartService)
        {
            this.cartService = cartService;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetCart(string username)
        {
            return Ok(await cartService.GetCartAsync(username));
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart(string username, string maSach, int soLuong)
        {
            return Ok(await cartService.AddToCartAsync(username, maSach, soLuong));
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateQuantity(string username, string maSach, int soLuong)
        {
            return Ok(await cartService.UpdateQuantityAsync(username, maSach, soLuong));
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveItem(string username, string maSach)
        {
            var result = await cartService.RemoveItemAsync(username, maSach);
            return result ? Ok() : NotFound();
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout(string username)
        {
            var data = await cartService.CheckoutAsync(username);
            return Ok(data);
        }
    }

}
