using Shopping.DataAccess.Models;

namespace shopping.DataAccess.IRepositories
{
    public interface IProductRepository
    {
        Product GetById(int productId);
        void Update(Product product);
    }
}
