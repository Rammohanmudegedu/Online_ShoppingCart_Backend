using Microsoft.AspNetCore.Mvc;
using Shopping.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using shopping.application.Iservices;

namespace Online_ShoppingCart_API.Controllers
{
    [Route("api/v1/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] OrderItem cartItem)
        {
            var (success, message) = await _cartService.AddToCartAsync(cartItem);
            if (success) return Ok(message);
            return BadRequest(message);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var cart = await _cartService.GetCartAsync();
            return Ok(cart);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("{email}")]
        public async Task<IActionResult> GetUserCart(string email)
        {
            var userCartItems = await _cartService.GetUserCartAsync(email);
            return Ok(userCartItems);
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{email}/{productId}")]
        public async Task<IActionResult> DeleteCartItem(string email, int productId)
        {
            var (success, message) = await _cartService.DeleteCartItemAsync(email, productId);
            if (success) return Ok(message);
            return NotFound(message);
        }

    }
}
