using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order = Shopping.DataAccess.Models.Order;
using shopping.application.Iservices;

namespace Online_ShoppingCart_API.Controllers
{
    [Route("api/v1/order")]
    [ApiController]

    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult PlaceOrder(Order order)
        {
            var (success, message) = _orderService.PlaceOrder(order);
            if (success) return Ok(message);
            return BadRequest(message);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            try
            {
                var orders = await _orderService.GetOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("{orderId}")]
        public async Task<ActionResult<Order>> GetOrderById(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                if (order == null) return NotFound("Order not found");
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("{email}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByEmail(string email)
        {
            try
            {
                var orders = await _orderService.GetOrdersByEmailAsync(email);
                if (orders == null || !orders.Any()) return NotFound("No orders found for the user");
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "User")]
        [HttpDelete("{orderId}")]
        public async Task<ActionResult> CancelOrderById(int orderId)
        {
            var (success, message) = await _orderService.CancelOrderByIdAsync(orderId);
            if (success) return Ok(message);
            return NotFound(message);
        }

    }
}
