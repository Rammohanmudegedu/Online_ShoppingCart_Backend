using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MailChimp.Net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Online_ShoppingCart_API.Controllers;
using Online_ShoppingCart_API.IServices;
using Online_ShoppingCart_API.Models;
using Xunit;
using Product = Online_ShoppingCart_API.Models.Product;

namespace Online_ShoppingCart_API.Test.Controllers
{
    public class ProductsControllerTests
    {
        private readonly DbContextOptions<StoreContext> _dbContextOptions;

        public ProductsControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private List<Product> GetTestProducts()
        {
            return new List<Product>
            {
                new Product { ProductId = 1, Product_Name = "Product", Description="description", Brand_Name = "Brand", Category = "Category", UnitPrice = 10, QuantityInStock=10, Product_Image=[] },
                new Product { ProductId = 2, Product_Name = "Product", Description="description", Brand_Name = "Brand", Category = "Category", UnitPrice = 20, QuantityInStock=10, Product_Image=[] },
                new Product { ProductId = 3, Product_Name = "Product", Description="description", Brand_Name = "Brand", Category = "Category", UnitPrice = 30, QuantityInStock=10, Product_Image=[] }
            };
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult()
        {
            // Arrange
            var products = GetTestProducts();
            using (var context = new StoreContext(_dbContextOptions))
            {
                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }

            using (var context = new StoreContext(_dbContextOptions))
            {
                var controller = new ProductsController(context);

                // Act
                var result = await controller.GetProducts();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnedProducts = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
                Assert.Equal(products.Count, returnedProducts.Count());
            }
        }

        [Fact]
        public async Task GetProduct_ReturnsOkResult()
        {
            // Arrange
            var products = GetTestProducts();
            using (var context = new StoreContext(_dbContextOptions))
            {
                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }

            using (var context = new StoreContext(_dbContextOptions))
            {
                var controller = new ProductsController(context);

                // Act
                var result = await controller.Getproduct(1);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var product = Assert.IsType<Product>(okResult.Value);
                Assert.Equal(products[0].Product_Name, product.Product_Name);
            }
        }


        [Fact]
        public async Task AddProduct_ReturnsOkResult()
        {
            // Arrange
            var product = new Product { ProductId = 1, Product_Name = "Product", Description = "description", Brand_Name = "Brand", Category = "Category", UnitPrice = 10, QuantityInStock = 10, Product_Image = [] };
            var imageBytes = new byte[] { 0x00, 0x01, 0x02 };
            var formFile = new FormFile(new MemoryStream(imageBytes), 0, imageBytes.Length, "ProductImage", "test.jpg");

            using (var context = new StoreContext(_dbContextOptions))
            {
                var controller = new ProductsController(context);

                // Act
                var result = await controller.AddProduct(product, formFile);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal("Product Added Successfully", okResult.Value);
            }
        }




        [Fact]
        public async Task DeleteProduct_ReturnsOkResult()
        {
            // Arrange
            var productId = 1;
            var options = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            
            using (var context = new StoreContext(options))
            {
                context.Products.Add(new Product { ProductId = productId, Product_Name = "Product", Description = "description", Brand_Name = "Brand", Category = "Category", UnitPrice = 10, QuantityInStock = 10, Product_Image = [] });
                await context.SaveChangesAsync();
            }

            using (var context = new StoreContext(options))
            {
                var controller = new ProductsController(context);

                // Act
                var result = await controller.deleteProduct(productId);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal("Product Deleted Successfully", okResult.Value);

              
                Assert.Null(context.Products.Find(productId));
            }
        }








    }
}
