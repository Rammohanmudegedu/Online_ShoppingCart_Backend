using Shopping.DataAccess.Models;

namespace shopping.DataAccess.IRepositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int productId);
        Task UpdateAsync(Product product);
    }
}
