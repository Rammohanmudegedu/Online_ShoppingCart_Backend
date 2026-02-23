using MailChimp.Net.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Shopping.Application;
using Shopping.DataAccess.Models;
using System.Security.Claims;
using Order = Online_ShoppingCart_API.Models.Order;

namespace Online_ShoppingCart_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class OrderController : Controller
    {

        private readonly StoreContext _storecontext;


        public OrderController(StoreContext storeContext)
        {
            _storecontext = storeContext;

        }

        [Authorize(Roles = "User")]
        [HttpPost("PlaceOrder")]
        public IActionResult PlaceOrder([FromServices] IHttpContextAccessor httpContextAccessor, Order order, [FromServices] IProductService productService)
        {
            try
            {
                /*var session = httpContextAccessor.HttpContext.Session;

                var cartJson = session.GetString("Cart");
                var cart = string.IsNullOrEmpty(cartJson) ? new List<OrderItem>() : JsonConvert.DeserializeObject<List<OrderItem>>(cartJson);

                var orders = cart.Where(item => item.Email == email).ToList();

                if (orders.Count == 0)
                {
                    return BadRequest("there is no cart items for this user.");
                }*/
                var orders = order.OrderItems.ToList();

                decimal orderPrice = orders.Sum(item => item.UnitPrice * item.Quantity);


                foreach (var item in orders)
                {
                    var product = productService.GetProductById(item.ProductId);
                    if (product != null)
                    {
                        if (product.QuantityInStock < item.Quantity)
                        {
                            return BadRequest($"{product.Product_Name} is not in stock.");
                        }

                        product.QuantityInStock -= item.Quantity;
                        productService.UpdateProduct(product);
                    }
                }

                var newOrder = new Order
                {
                    Address = order.Address,
                    ZipCode = order.ZipCode,
                    City = order.City,
                    OrderPrice = orderPrice,
                    OrderedDate = order.OrderedDate,
                    OrderStatus = "Active",
                    OrderItems = orders,

                };


                _storecontext.Orders.Add(newOrder);
                _storecontext.SaveChanges();


                /*cart.RemoveAll(item => item.Email == email);
                session.SetString("Cart", JsonConvert.SerializeObject(cart));*/

                return Ok("Order placed successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }







        [Authorize(Roles = "Admin")]
        [HttpGet("GetOrders")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            try
            {
                if (_storecontext.Orders == null)
                {
                    return NotFound();
                }
                var order = _storecontext.Orders
                    .Include(o => o.OrderItems); 
       

                return Ok(await order.ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetOrderById/{orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(int orderId)
        {
            try
            {
                var order = await _storecontext.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    return NotFound("Order not found");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetOrdersByEmail/{email}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByEmail(string email)
        {
            try
            {
                var orders = await _storecontext.Orders
                    .Include(o => o.OrderItems)
                    .Where(o => o.OrderItems.Any(item => item.Email == email))
                    .ToListAsync();

                if (orders == null || !orders.Any())
                {
                    return NotFound("No orders found for the user");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "User")]
        [HttpDelete("cancelOrderById/{orderId}")]
        public async Task<ActionResult> CancelOrderById(int orderId)
        {
            try
            {
                var order = await _storecontext.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId);

                if (order == null)
                {
                    return NotFound("Order not found");
                }

                if (order.OrderStatus != "Cancelled")
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        var product = await _storecontext.Products.FirstOrDefaultAsync(p => p.ProductId == orderItem.ProductId);
                        if (product != null)
                        {
                            product.QuantityInStock += orderItem.Quantity;
                        }
                    }

                    order.OrderStatus = "Cancelled";
                    await _storecontext.SaveChangesAsync();
                }

                return Ok("Order cancelled successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }






    }
}
