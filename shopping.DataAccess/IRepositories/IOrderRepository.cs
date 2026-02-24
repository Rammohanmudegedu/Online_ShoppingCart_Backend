using Shopping.DataAccess.Models;

namespace shopping.DataAccess.IRepositories
{
    public interface IOrderRepository
    {
        void Add(Order order);
        IQueryable<Order> GetAll();
        Task<Order?> GetByIdAsync(int orderId);
        Task<List<Order>> GetByEmailAsync(string email);
        void Update(Order order);
        Task SaveChangesAsync();
    }
}
