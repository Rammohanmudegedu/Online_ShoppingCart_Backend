using System.Linq;
using shopping.DataAccess.IRepositories;
using Shopping.DataAccess.Models;

namespace shopping.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _storeContext;

        public ProductRepository(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public Product GetById(int productId)
        {
            return _storeContext.Products.FirstOrDefault(p => p.ProductId == productId);
        }

        public void Update(Product product)
        {
            _storeContext.Products.Update(product);
            _storeContext.SaveChanges();
        }
    }
}
