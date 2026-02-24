using Microsoft.AspNetCore.Authorization;
using Online_ShoppingCart_API.Auth;
using Online_ShoppingCart_API.Service;
using shopping.application.Iservices;
using shopping.DataAccess.IRepositories;
using shopping.DataAccess.Repositories;

namespace Online_ShoppingCart_API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        //Register services
        public static IServiceCollection AddAllServices(this IServiceCollection services)
        {
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IProductService, ProductService>();

            return services;
        }

        //Register repositories
        public static IServiceCollection AddAllRepositories(this IServiceCollection repositories)
        {
            repositories.AddScoped<IProductRepository, ProductRepository>();
            repositories.AddScoped<IOrderRepository, OrderRepository>();
            repositories.AddScoped<IAuthRepository, AuthRepository>();

            return repositories;
        }
    }

}
