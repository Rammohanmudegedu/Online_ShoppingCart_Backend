using Shopping.DataAccess.Models;

namespace shopping.application.Iservices
{
    public interface IProductService
    {
        Product GetProductById(int productId);
        void UpdateProduct(Product product);
    }
}
