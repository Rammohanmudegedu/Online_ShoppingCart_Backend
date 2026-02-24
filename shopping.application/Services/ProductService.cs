using Microsoft.EntityFrameworkCore;
using shopping.application.Iservices;
using Shopping.DataAccess.Models;

namespace Online_ShoppingCart_API.Service
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
