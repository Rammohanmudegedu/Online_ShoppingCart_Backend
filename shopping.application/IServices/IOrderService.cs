using Shopping.DataAccess.Models;

namespace shopping.application.Iservices
{
    public interface IOrderService
    {
        Task<(bool Success, string Message)> PlaceOrderAsync(Order order);
        Task<List<Order>> GetOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<List<Order>> GetOrdersByEmailAsync(string email);
        Task<(bool Success, string Message)> CancelOrderByIdAsync(int orderId);
    }
}
