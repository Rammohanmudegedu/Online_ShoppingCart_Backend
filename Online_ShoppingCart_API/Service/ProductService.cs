using Microsoft.EntityFrameworkCore;
using Online_ShoppingCart_API.IServices;
using Online_ShoppingCart_API.Models;
using Product = Online_ShoppingCart_API.Models.Product;

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
