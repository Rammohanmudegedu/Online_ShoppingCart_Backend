using Shopping.DataAccess.Models;

namespace shopping.application.Iservices
{
    public interface ICartService
    {
        Task<(bool Success, string Message)> AddToCartAsync(OrderItem cartItem);
        Task<List<OrderItem>> GetCartAsync();
        Task<List<OrderItem>> GetUserCartAsync(string email);
        Task<(bool Success, string Message)> DeleteCartItemAsync(string email, int productId);
    }
}
