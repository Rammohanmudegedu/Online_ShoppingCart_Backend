﻿using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Online_ShoppingCart_API.Models;
using Online_ShoppingCart_API.Repository;
using System.Net.WebSockets;

namespace Online_ShoppingCart_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        

        private readonly IJWTManagerRepository _jWTManager;

        private readonly UserManager<IdentityUser> _userManager;

     


        public UsersController(IJWTManagerRepository jWTManager, UserManager<IdentityUser> userManager)
        {

            _jWTManager = jWTManager;
            _userManager = userManager;
         
        }
        
        [Authorize(Roles = "Admin")]
        [HttpGet("getAllUsers")]
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
        [HttpGet("getUser/{email}")]
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
        [HttpPost("addUser")]
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
        [HttpPut("updateUser/{email}")]
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
        [HttpDelete("deleteUser/{email}")]
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



        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public async Task<IActionResult> Authenticate(LoginModel login)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(login.Email);

                if (user != null)
                {
                    var checkPassword = await _userManager.CheckPasswordAsync(user, login.Password);

                    if (checkPassword)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles != null)
                        {
                            var jwtToken = _jWTManager.CreateJWTToken(user, roles.ToList());

                            var response = new Tokens
                            {
                                Token = jwtToken,
                                Roles = roles.ToList()
                               
                        };

                            return Ok(response);

                        }
                    } 
                }
                return BadRequest("Email or Password Incorrect.");
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
            
        }
    }
}
