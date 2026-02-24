using Shopping.DataAccess.Models;
using Microsoft.AspNetCore.Identity;

namespace shopping.application.Iservices
{
    public interface IUserService
    {
        Task<List<IdentityUser>> GetAllUsersAsync();
        Task<IdentityUser?> GetUserByEmailAsync(string email);
        Task<(bool Success, string Message)> AddUserAsync(User user);
        Task<(bool Success, string Message)> UpdateUserAsync(User user, string email);
        Task<(bool Success, string Message)> DeleteUserAsync(string email);
    }
}