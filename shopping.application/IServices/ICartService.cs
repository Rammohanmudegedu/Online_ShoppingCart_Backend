using Shopping.DataAccess.Models;

namespace shopping.application.Iservices
{
    public interface ICartService
    {
        (bool Success, string Message) AddToCart(OrderItem cartItem);
        List<OrderItem> GetCart();
        List<OrderItem> GetUserCart(string email);
        (bool Success, string Message) DeleteCartItem(string email, int productId);
    }
}
