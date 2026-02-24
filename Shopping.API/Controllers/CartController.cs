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
        public IActionResult AddToCart([FromBody] OrderItem cartItem)
        {
            var (success, message) = _cartService.AddToCart(cartItem);
            if (success) return Ok(message);
            return BadRequest(message);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetCart()
        {
            var cart = _cartService.GetCart();
            return Ok(cart);
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("{email}")]
        public IActionResult GetUserCart(string email)
        {
            var userCartItems = _cartService.GetUserCart(email);
            return Ok(userCartItems);
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{email}/{productId}")]
        public IActionResult DeleteCartItem(string email, int productId)
        {
            var (success, message) = _cartService.DeleteCartItem(email, productId);
            if (success) return Ok(message);
            return NotFound(message);
        }

    }
}
