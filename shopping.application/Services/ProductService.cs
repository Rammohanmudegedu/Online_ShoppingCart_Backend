using Microsoft.EntityFrameworkCore;
using shopping.application.Iservices;
using Shopping.DataAccess.Models;
using shopping.DataAccess.IRepositories;

namespace Online_ShoppingCart_API.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateAsync(product);
        }
    }

}
