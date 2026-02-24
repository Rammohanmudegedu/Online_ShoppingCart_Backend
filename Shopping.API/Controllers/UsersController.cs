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
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();

                if (users != null && users.Any())
                {
                    
                    return Ok(users);
                }
                else
                {
                    return NotFound("No users found");
                }
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
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                   
                    return Ok(user);
                }
                else
                {
                    return NotFound("User not found");
                }
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
            try
            {
                
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    return Conflict("User with this email already exists");
                }

                
                    var identityUser = new IdentityUser
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.Phone
                    };

                    var identityResult = await _userManager.CreateAsync(identityUser, user.Password);
                    if (identityResult.Succeeded)
                    {
                        if (user.Roles != null && user.Roles.Any())
                        {
                            var userEmail = await _userManager.FindByEmailAsync(user.Email);



                            string[] defaultRoles = user.Roles;

                            identityResult = await _userManager.AddToRolesAsync(userEmail, defaultRoles);

                            if (identityResult.Succeeded)
                            {


                                return Ok("User Added Successfully");
                            }
                        }
                    }

                

                return BadRequest("Failed to add user");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "User,Admin")]
        [HttpPut("{email}")]
        public async Task<IActionResult> UpdateUser(User user, string email)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingUser = await _userManager.FindByEmailAsync(email);

                    if (existingUser != null)
                    {
                        existingUser.UserName = user.UserName;
                        existingUser.PhoneNumber = user.Phone;
                        existingUser.Email = user.Email;

                        var identityResult = await _userManager.UpdateAsync(existingUser);

                        if (identityResult.Succeeded)
                        {
                            // update user roles if need

                            if (user.Roles != null && user.Roles.Any())
                            {
                                var rolesAsString = string.Join(",", user.Roles);

                                identityResult = await _userManager.RemoveFromRolesAsync(existingUser, await _userManager.GetRolesAsync(existingUser));
                                if (identityResult.Succeeded)
                                {
                                    identityResult = await _userManager.AddToRoleAsync(existingUser, rolesAsString);

                                    if (identityResult.Succeeded)
                                    {
                                        return Ok("User Updated Successfully");
                                    }
                                }
                            }
                            else
                            {
                                return Ok("User Updated Successfully");
                            }
                        }
                    }
                    else
                    {
                        return NotFound("User not found");
                    }
                }

                return BadRequest("Invalid user data");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUser(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var result = await _userManager.DeleteAsync(user);

                    if (result.Succeeded)
                    {
                        return Ok("User deleted successfully");
                    }
                   
                }
                return NotFound("User not found");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
