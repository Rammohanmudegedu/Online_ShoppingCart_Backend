using MailChimp.Net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Online_ShoppingCart_API.Models;
using Newtonsoft.Json;
using System;
using System.Text.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using Microsoft.EntityFrameworkCore;
using Order = Online_ShoppingCart_API.Models.Order;
using Online_ShoppingCart_API.IServices;
using Microsoft.AspNetCore.Authorization;

namespace Online_ShoppingCart_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CartController : ControllerBase
    {
        private readonly StoreContext _storecontext;


        public CartController(StoreContext storeContext)
        {
            _storecontext = storeContext;

        }


        [Authorize(Roles = "User")]
        [HttpPost("AddToCart")]
        public IActionResult AddToCart([FromBody] OrderItem cartItem, [FromServices] IHttpContextAccessor httpContextAccessor, [FromServices] IProductService productService)
        {
            try
            {
                var session = httpContextAccessor.HttpContext.Session;

             
                var cartJson = session.GetString("Cart");
                var cart = string.IsNullOrEmpty(cartJson) ? new List<OrderItem>() : JsonConvert.DeserializeObject<List<OrderItem>>(cartJson);

            
                var product = productService.GetProductById(cartItem.ProductId);

                if (product != null)
                {

                    if (product.QuantityInStock < cartItem.Quantity)
                    {
                        return BadRequest("Insufficient quantity in stock.");
                    }

                    if (cartItem.Quantity <= 0)
                    {
                        cartItem.Quantity = 1;
                    }

                    var existingItem = cart.FirstOrDefault(item => item.ProductId == cartItem.ProductId && item.Email == cartItem.Email);

                    if (existingItem != null)
                    {
                        
                        existingItem.Quantity += cartItem.Quantity;
                    }

                    else
                    {
                        
                        cartItem.UnitPrice = product.UnitPrice;
                        cartItem.Product_Image = product.Product_Image;
                        cartItem.Product_Name = product.Product_Name;

                        cart.Add(cartItem);
                    }

                    
                    session.SetString("Cart", JsonConvert.SerializeObject(cart));

                    return Ok("Item added to cart");
                }
                else
                {
                    return BadRequest("Product not found");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetCart")]
        public IActionResult GetCart([FromServices] IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var session = httpContextAccessor.HttpContext.Session;
                var cartJson = session?.GetString("Cart");
                var cart = string.IsNullOrEmpty(cartJson) ? new List<OrderItem>() : JsonConvert.DeserializeObject<List<OrderItem>>(cartJson);


                return Ok(cart);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetUserCart/{email}")]
        public IActionResult GetUserCart(string email, [FromServices] IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var session = httpContextAccessor.HttpContext.Session;
                var cartJson = session.GetString("Cart");

                var cart = string.IsNullOrEmpty(cartJson) ? new List<OrderItem>() : JsonConvert.DeserializeObject<List<OrderItem>>(cartJson);

                var userCartItems = cart.Where(item => item.Email == email).ToList();

                return Ok(userCartItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize(Roles = "User")]
        [HttpDelete("DeleteCartItem/{email}/{productId}")]
        public IActionResult DeleteCartItem(string email, int productId, [FromServices] IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var session = httpContextAccessor.HttpContext.Session;
                var cartJson = session.GetString("Cart");

                if (string.IsNullOrEmpty(cartJson))
                {
                    return NotFound("Cart is empty");
                }

                var cart = JsonConvert.DeserializeObject<List<OrderItem>>(cartJson);

                
                var itemToRemove = cart.FirstOrDefault(item => item.ProductId == productId && item.Email == email);

               

                
                cart.Remove(itemToRemove);

                
                session.SetString("Cart", JsonConvert.SerializeObject(cart));

                return Ok("Item removed from the cart");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }




    }


}
