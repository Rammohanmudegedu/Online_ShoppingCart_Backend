using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using shopping.application.Iservices;
using shopping.DataAccess.IRepositories;
using Shopping.DataAccess.Models;

namespace Online_ShoppingCart_API.Service
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductRepository _productRepository;

        public CartService(IHttpContextAccessor httpContextAccessor, IProductRepository productRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _productRepository = productRepository;
        }

        private List<OrderItem> ReadCart()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
                return new List<OrderItem>();

            if (!session.TryGetValue("Cart", out var cartBytes) || cartBytes == null || cartBytes.Length == 0)
                return new List<OrderItem>();

            try
            {
                var cartJson = Encoding.UTF8.GetString(cartBytes);
                return string.IsNullOrEmpty(cartJson) ? new List<OrderItem>() : JsonConvert.DeserializeObject<List<OrderItem>>(cartJson)!;
            }
            catch
            {
                return new List<OrderItem>();
            }
        }

        private void SaveCart(List<OrderItem> cart)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            if (session == null)
                return;

            var json = JsonConvert.SerializeObject(cart);
            var bytes = Encoding.UTF8.GetBytes(json);
            session.Set("Cart", bytes);
        }

        public async Task<(bool Success, string Message)> AddToCartAsync(OrderItem cartItem)
        {
            try
            {
                var cart = ReadCart();

                var product = await _productRepository.GetByIdAsync(cartItem.ProductId);

                if (product == null)
                    return (false, "Product not found");

                if (product.QuantityInStock < cartItem.Quantity)
                    return (false, "Insufficient quantity in stock.");

                if (cartItem.Quantity <= 0)
                    cartItem.Quantity = 1;

                var existingItem = cart.FirstOrDefault(item => item.ProductId == cartItem.ProductId && item.Email == cartItem.Email);

                if (existingItem != null)
                {
                    existingItem.Quantity += cartItem.Quantity;
                }
                else
                {
                    cartItem.UnitPrice = product.UnitPrice;
                    cartItem.Product_Image = product.Product_Image;
                    cartItem.Product_Name = product.Product_Name;
                    cart.Add(cartItem);
                }

                SaveCart(cart);
                return (true, "Item added to cart");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public Task<List<OrderItem>> GetCartAsync()
        {
            try
            {
                return Task.FromResult(ReadCart());
            }
            catch
            {
                return Task.FromResult(new List<OrderItem>());
            }
        }

        public Task<List<OrderItem>> GetUserCartAsync(string email)
        {
            try
            {
                var cart = ReadCart();
                return Task.FromResult(cart.Where(i => i.Email == email).ToList());
            }
            catch
            {
                return Task.FromResult(new List<OrderItem>());
            }
        }

        public Task<(bool Success, string Message)> DeleteCartItemAsync(string email, int productId)
        {
            try
            {
                var cart = ReadCart();
                var itemToRemove = cart.FirstOrDefault(item => item.ProductId == productId && item.Email == email);
                if (itemToRemove == null)
                    return Task.FromResult((false, "Item not found in cart"));

                cart.Remove(itemToRemove);
                SaveCart(cart);
                return Task.FromResult((true, "Item removed from the cart"));
            }
            catch (Exception ex)
            {
                return Task.FromResult((false, ex.Message));
            }
        }
    }
}
