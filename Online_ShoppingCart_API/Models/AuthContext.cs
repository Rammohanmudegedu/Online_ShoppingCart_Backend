using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Online_ShoppingCart_API.Models
{
    public class AuthContext : IdentityDbContext
    {
        public AuthContext(DbContextOptions<AuthContext> options) : base(options)
        { 
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var UserRoleId = "0e45f524-8c5d-40df-beb3-a08545a5bbc9";
            var AdminRoleId = "a68578e9-8f43-43a5-9953-f7c6ca99dfbd";
            var SupplierRoleId = "7d332c08-0d02-483b-b677-a5ceb20be8f5";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = UserRoleId,
                    ConcurrencyStamp = UserRoleId,
                    Name = "User",
                    NormalizedName = "User".ToUpper()
                },
                new IdentityRole
                { 
                    Id = AdminRoleId,
                    ConcurrencyStamp = AdminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                },
                new IdentityRole 
                { 
                    Id = SupplierRoleId,
                    ConcurrencyStamp = SupplierRoleId,
                    Name = "Supplier",
                    NormalizedName = "Supplier".ToUpper()
                }

            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
