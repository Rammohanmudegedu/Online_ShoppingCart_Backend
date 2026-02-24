using Microsoft.AspNetCore.Identity;
using Shopping.DataAccess.Models;

namespace shopping.DataAccess.IRepositories
{
    public interface IUserRepository
    {
        Task<List<IdentityUser>> GetAllUsersAsync();
        Task<IdentityUser?> FindByEmailAsync(string email);
        Task<IdentityResult> CreateAsync(IdentityUser user, string password);
        Task<IdentityResult> UpdateAsync(IdentityUser user);
        Task<IdentityResult> DeleteAsync(IdentityUser user);
        Task<IdentityResult> AddToRolesAsync(IdentityUser user, IEnumerable<string> roles);
        Task<IdentityResult> RemoveFromRolesAsync(IdentityUser user, IEnumerable<string> roles);
        Task<IList<string>> GetRolesAsync(IdentityUser user);
    }
}