using Microsoft.EntityFrameworkCore;
using shopping.DataAccess.IRepositories;
using Shopping.DataAccess.Models;

namespace shopping.DataAccess.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly StoreContext _storeContext;

        public OrderRepository(StoreContext storeContext)
        {
            _storeContext = storeContext;
        }

        public void Add(Order order)
        {
            _storeContext.Orders.Add(order);
        }

        public IQueryable<Order> GetAll()
        {
            return _storeContext.Orders.Include(o => o.OrderItems);
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _storeContext.Orders.Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<List<Order>> GetByEmailAsync(string email)
        {
            return await _storeContext.Orders.Include(o => o.OrderItems)
                .Where(o => o.OrderItems.Any(item => item.Email == email)).ToListAsync();
        }

        public void Update(Order order)
        {
            _storeContext.Orders.Update(order);
        }

        public async Task SaveChangesAsync()
        {
            await _storeContext.SaveChangesAsync();
        }
    }
}
