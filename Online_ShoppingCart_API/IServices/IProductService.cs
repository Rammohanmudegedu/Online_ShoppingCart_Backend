using Online_ShoppingCart_API.Models;

namespace Online_ShoppingCart_API.IServices
{
    public interface IProductService
    {
        Product GetProductById(int productId);

        void UpdateProduct(Product product);
    }
}
