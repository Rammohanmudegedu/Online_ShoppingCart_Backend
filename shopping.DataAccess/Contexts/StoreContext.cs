using Microsoft.EntityFrameworkCore;

namespace Shopping.DataAccess.Models
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
                .HasOne<Product>()
                .WithMany()
                .HasForeignKey(p => p.ProductId);

            modelBuilder.Entity<OrderItem>()
                .HasKey(ci => new { ci.OrderId, ci.ProductId, ci.Email});

            modelBuilder.Entity<OrderItem>()
                .HasOne<Order>()
                .WithMany(o => o.OrderItems)
                .HasForeignKey(ci => ci.OrderId);
        }
    }
}
