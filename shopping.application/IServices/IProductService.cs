using Shopping.DataAccess.Models;

namespace shopping.application.Iservices
{
    public interface IProductService
    {
        Task<Product?> GetProductByIdAsync(int productId);
        Task UpdateProductAsync(Product product);
    }
}
