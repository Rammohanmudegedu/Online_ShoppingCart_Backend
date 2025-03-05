using MailChimp.Net.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Online_ShoppingCart_API.Controllers;
using Online_ShoppingCart_API.IServices;
using Online_ShoppingCart_API.Models;
using Online_ShoppingCart_API.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Order = Online_ShoppingCart_API.Models.Order;

namespace Online_ShoppingCart_API.Test.Controllers
{
    public class OrderControllerTests
    {
        private readonly DbContextOptions<StoreContext> _dbContextOptions;
  

        public OrderControllerTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<StoreContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private List<Order> GetTestOrders()
        {
            return new List<Order>
            {
                new Order { OrderId=1, Address="Address", ZipCode="123456", City="city", OrderItems=[], OrderPrice=20000, OrderedDate=DateTime.Now, OrderStatus="Active" }

            };
        }


        [Fact]
        public async Task GetOrders_ReturnsOkResult()
        {
            // Arrange
            var orders = GetTestOrders();
            using (var context = new StoreContext(_dbContextOptions))
            {
                await context.Orders.AddRangeAsync(orders);
                await context.SaveChangesAsync();
            }

            using (var context = new StoreContext(_dbContextOptions))
            {
                var controller = new OrderController(context);

                // Act
                var result = await controller.GetOrders();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnedOrders = Assert.IsAssignableFrom<IEnumerable<Order>>(okResult.Value);
                Assert.Equal(orders.Count, returnedOrders.Count());
            }
        }

        [Fact]
        public async Task GetOrderById_ReturnsOkResult()
        {
            // Arrange
            var orders = GetTestOrders();
            using (var context = new StoreContext(_dbContextOptions))
            {
                await context.Orders.AddRangeAsync(orders);
                await context.SaveChangesAsync();
            }

            using (var context = new StoreContext(_dbContextOptions))
            {
                var controller = new OrderController(context);

                // Act
                var result = await controller.GetOrderById(1);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                var returnedOrders = Assert.IsType<Order>(okResult.Value);
                Assert.Equal(orders[0].OrderId, returnedOrders.OrderId);
            }
        }


        



    }
}
