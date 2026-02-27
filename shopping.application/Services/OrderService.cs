using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using shopping.application.Iservices;
using shopping.DataAccess.IRepositories;
using Shopping.DataAccess.Models;
using Shopping.Utilities.Common;

namespace Online_ShoppingCart_API.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<(bool Success, string Message)> PlaceOrderAsync(Order order)
        {
            try
            {
                var orders = order.OrderItems.ToList();
                decimal orderPrice = orders.Sum(item => item.UnitPrice * item.Quantity);

                foreach (var item in orders)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null)
                        return (false, "Product not found");

                    if (product.QuantityInStock < item.Quantity)
                        return (false, $"{product.Product_Name} is not in stock.");

                    product.QuantityInStock -= item.Quantity;
                    await _productRepository.UpdateAsync(product);
                }

                var newOrder = new Order
                {
                    Address = order.Address,
                    ZipCode = order.ZipCode,
                    City = order.City,
                    OrderPrice = orderPrice,
                    OrderedDate = order.OrderedDate,
                    OrderStatus = "Active",
                    OrderItems = orders,
                };

                _orderRepository.Add(newOrder);
                await _orderRepository.SaveChangesAsync();

                // Try to send order confirmation email(s) to user(s) associated with the order items.
                try
                {
                    var (subject, body) = BuildOrderEmail(newOrder);

                    var recipientEmails = newOrder.OrderItems
                        .Where(i => !string.IsNullOrWhiteSpace(i.Email))
                        .Select(i => i.Email.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    foreach (var to in recipientEmails)
                    {
                        using MailMessage message = new();
                        message.To.Add(to);
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = true;
                        message.Priority = MailPriority.Normal;

                        SMTPClient.SendEmail(message);
                    }
                }
                catch
                {
                   
                }

                return (true, "Order placed successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<List<Order>> GetOrdersAsync()
        {
            return await _orderRepository.GetAll().ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetByIdAsync(orderId);
        }

        public async Task<List<Order>> GetOrdersByEmailAsync(string email)
        {
            return await _orderRepository.GetByEmailAsync(email);
        }

        public async Task<(bool Success, string Message)> CancelOrderByIdAsync(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return (false, "Order not found");

                if (order.OrderStatus != "Cancelled")
                {
                    foreach (var orderItem in order.OrderItems)
                    {
                        var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
                        if (product != null)
                        {
                            product.QuantityInStock += orderItem.Quantity;
                            await _productRepository.UpdateAsync(product);
                        }
                    }

                    order.OrderStatus = "Cancelled";
                    _orderRepository.Update(order);
                    await _orderRepository.SaveChangesAsync();
                }

                return (true, "Order cancelled successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // Build email subject and HTML body for the placed order
        private (string Subject, string Body) BuildOrderEmail(Order order)
        {
            var subject = $"Order Confirmation - Order #{order.OrderId}";

            var sb = new StringBuilder();
            sb.AppendLine("<h2>Thank you for your order!</h2>");
            sb.AppendLine($"<p>Order Id: <strong>{order.OrderId}</strong></p>");
            sb.AppendLine($"<p>Order Date: <strong>{order.OrderedDate:dddd, dd MMM yyyy HH:mm}</strong></p>");
            sb.AppendLine($"<p>Order Status: <strong>{order.OrderStatus}</strong></p>");
            sb.AppendLine("<h3>Shipping Details</h3>");
            sb.AppendLine($"<p>{order.Address}, {order.City} - {order.ZipCode}</p>");

            sb.AppendLine("<h3>Items</h3>");
            sb.AppendLine("<table border=1 cellpadding=5 cellspacing=0 style='border-collapse:collapse;'>");
            sb.AppendLine("<thead><tr><th>Image</th><th>Product</th><th>Qty</th><th>Unit Price</th><th>Total</th></tr></thead>");
            sb.AppendLine("<tbody>");

            foreach (var item in order.OrderItems)
            {
                var lineTotal = item.UnitPrice * item.Quantity;
                sb.AppendLine("<tr>");
                // Image cell (embed as base64 data URI when available)
                if (item.Product_Image != null && item.Product_Image.Length > 0)
                {
                    // embed image as base64 data URI so it can be displayed in the email
                    string base64 = Convert.ToBase64String(item.Product_Image);
                    string img = $"data:image/jpeg;base64,{base64}";
                    sb.AppendLine($"<td><img src=\"{img}\" alt=\"{System.Net.WebUtility.HtmlEncode(item.Product_Name)}\" style=\"max-width:120px;max-height:120px;object-fit:contain;\"/></td>");
                }
                else
                {
                    sb.AppendLine("<td></td>");
                }

                sb.AppendLine($"<td>{System.Net.WebUtility.HtmlEncode(item.Product_Name)}</td>");
                sb.AppendLine($"<td align=right>{item.Quantity}</td>");
                sb.AppendLine($"<td align=right>{item.UnitPrice:C}</td>");
                sb.AppendLine($"<td align=right>{lineTotal:C}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");

            sb.AppendLine($"<p><strong>Order Total: {order.OrderPrice:C}</strong></p>");

            sb.AppendLine("<h4>Notes</h4>");
            sb.AppendLine("<p>Your order is being processed. If you have questions, reply to this email or contact support.</p>");

            // Add any additional helpful info
            sb.AppendLine("<hr>");
            sb.AppendLine($"<p style='font-size:0.9em;color:#666'>This is an automated message from {ResourceConstants.ApplicationName ?? "Shopping App"}.</p>");

            return (subject, sb.ToString());
        }
    }
}
