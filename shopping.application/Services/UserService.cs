using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using shopping.application.Iservices;
using shopping.DataAccess.IRepositories;
using Shopping.DataAccess.Models;

namespace Online_ShoppingCart_API.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userRepository;

        public UserService(UserManager<IdentityUser> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<IdentityUser>> GetAllUsersAsync()
        {
            return await _userRepository.Users.ToListAsync();
        }

        public async Task<IdentityUser?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.FindByEmailAsync(email);
        }

        public async Task<(bool Success, string Message)> AddUserAsync(User user)
        {
            try
            {
                var existing = await _userRepository.FindByEmailAsync(user.Email);
                if (existing != null)
                    return (false, "User with this email already exists");

                var identityUser = new IdentityUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.Phone
                };

                var result = await _userRepository.CreateAsync(identityUser, user.Password);
                if (!result.Succeeded)
                    return (false, string.Join(';', result.Errors.Select(e => e.Description)));

                if (user.Roles != null && user.Roles.Any())
                {
                    var roles = user.Roles;
                    var userFromDb = await _userRepository.FindByEmailAsync(user.Email);
                    if (userFromDb != null)
                    {
                        var roleResult = await _userRepository.AddToRolesAsync(userFromDb, roles);
                        if (!roleResult.Succeeded)
                            return (false, string.Join(';', roleResult.Errors.Select(e => e.Description)));
                    }
                }

                return (true, "User Added Successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> UpdateUserAsync(User user, string email)
        {
            try
            {
                var existingUser = await _userRepository.FindByEmailAsync(email);
                if (existingUser == null) return (false, "User not found");

                existingUser.UserName = user.UserName;
                existingUser.PhoneNumber = user.Phone;
                existingUser.Email = user.Email;

                var updateResult = await _userRepository.UpdateAsync(existingUser);
                if (!updateResult.Succeeded)
                    return (false, string.Join(';', updateResult.Errors.Select(e => e.Description)));

                if (user.Roles != null && user.Roles.Any())
                {
                    var currentRoles = await _userRepository.GetRolesAsync(existingUser);
                    var removeResult = await _userRepository.RemoveFromRolesAsync(existingUser, currentRoles);
                    if (!removeResult.Succeeded)
                        return (false, string.Join(';', removeResult.Errors.Select(e => e.Description)));

                    var addResult = await _userRepository.AddToRolesAsync(existingUser, user.Roles);
                    if (!addResult.Succeeded)
                        return (false, string.Join(';', addResult.Errors.Select(e => e.Description)));
                }

                return (true, "User Updated Successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(string email)
        {
            try
            {
                var existingUser = await _userRepository.FindByEmailAsync(email);
                if (existingUser == null) return (false, "User not found");

                var deleteResult = await _userRepository.DeleteAsync(existingUser);
                if (!deleteResult.Succeeded)
                    return (false, string.Join(';', deleteResult.Errors.Select(e => e.Description)));

                return (true, "User deleted successfully");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }
}