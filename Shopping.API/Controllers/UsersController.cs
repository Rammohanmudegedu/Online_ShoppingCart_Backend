using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shopping.DataAccess.IRepositories;
using Shopping.DataAccess.Models;

namespace Online_ShoppingCart_API.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly shopping.application.Iservices.IUserService _userService;

        public UsersController(shopping.application.Iservices.IUserService userService)
        {
            _userService = userService;
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                if (users != null && users.Any()) return Ok(users);
                return NotFound("No users found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "User,Admin")]
        [HttpGet("{email}")]
        public async Task<IActionResult> GetUser(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user != null) return Ok(user);
                return NotFound("User not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> addUser(User user)
        {
            var (success, message) = await _userService.AddUserAsync(user);
            if (success) return Ok(message);
            if (message.Contains("already exists")) return Conflict(message);
            return BadRequest(message);
        }


        [Authorize(Roles = "User,Admin")]
        [HttpPut("{email}")]
        public async Task<IActionResult> UpdateUser(User user, string email)
        {
            var (success, message) = await _userService.UpdateUserAsync(user, email);
            if (success) return Ok(message);
            if (message.Contains("not found")) return NotFound(message);
            return BadRequest(message);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            var (success, message) = await _userService.DeleteUserAsync(email);
            if (success) return Ok(message);
            return NotFound(message);
        }
    }
}
