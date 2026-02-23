using Shopping.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Shopping.Application
{
    public class ProductService : IProductService
    {
        private readonly StoreContext _storecontext;

        public ProductService(StoreContext storeContext)
        {
            _storecontext = storeContext;
        }

        public Product GetProductById(int productId)
        {
            return _storecontext.Products.FirstOrDefault(p => p.ProductId == productId);
        }

        public void UpdateProduct(Product product)
        {
            _storecontext.Products.Update(product);
            _storecontext.SaveChanges();
        }
    }
}
