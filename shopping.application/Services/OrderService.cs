using System.Linq;
using Microsoft.EntityFrameworkCore;
using shopping.application.Iservices;
using shopping.DataAccess.IRepositories;
using Shopping.DataAccess.Models;

namespace Online_ShoppingCart_API.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public (bool Success, string Message) PlaceOrder(Order order)
        {
            try
            {
                var orders = order.OrderItems.ToList();
                decimal orderPrice = orders.Sum(item => item.UnitPrice * item.Quantity);

                foreach (var item in orders)
                {
                    var product = _productRepository.GetById(item.ProductId);
                    if (product == null)
                        return (false, "Product not found");

                    if (product.QuantityInStock < item.Quantity)
                        return (false, $"{product.Product_Name} is not in stock.");

                    product.QuantityInStock -= item.Quantity;
                    _productRepository.Update(product);
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

                _orderRepository.Add(newOrder);
                _orderRepository.SaveChangesAsync().GetAwaiter().GetResult();

                return (true, "Order placed successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _orderRepository.GetAll().ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }

        public async Task<List<Order>> GetOrdersByEmailAsync(string email)
        {
            return await _orderRepository.GetByEmailAsync(email);
        }

        public async Task<(bool Success, string Message)> CancelOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return (false, "Order not found");

                if (order.OrderStatus != "Cancelled")
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        var product = _productRepository.GetById(orderItem.ProductId);
                        if (product != null)
                        {
                            product.QuantityInStock += orderItem.Quantity;
                            _productRepository.Update(product);
                        }
                    }

                    order.OrderStatus = "Cancelled";
                    _orderRepository.Update(order);
                    await _orderRepository.SaveChangesAsync();
                }

                return (true, "Order cancelled successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}
