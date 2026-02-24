using System.Linq;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Product?> GetByIdAsync(int productId)
        {
            return await _storeContext.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task UpdateAsync(Product product)
        {
            _storeContext.Products.Update(product);
            await _storeContext.SaveChangesAsync();
        }
    }
}
