using Shopping.DataAccess.Models;

namespace Shopping.Application
{
    public interface IProductService
    {
        Product GetProductById(int productId);
        void UpdateProduct(Product product);
    }
}
