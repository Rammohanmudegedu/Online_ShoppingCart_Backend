using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailChimp.Net.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Online_ShoppingCart_API.Controllers;
using Online_ShoppingCart_API.Models;
using Online_ShoppingCart_API.Repository;
using Xunit;
using IdentityResult = Microsoft.AspNetCore.Identity.IdentityResult;

namespace Online_ShoppingCart_API.Tests.Controllers
{

    public class UsersControllerTests
    {
        private readonly UsersController _controller;
        private readonly Mock<Microsoft.AspNetCore.Identity.UserManager<IdentityUser>> _userManagerMock;

        public UsersControllerTests()
        {
            _userManagerMock = new Mock<Microsoft.AspNetCore.Identity.UserManager<IdentityUser>>(
                Mock.Of<Microsoft.AspNetCore.Identity.IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null);

            _controller = new UsersController(new JWTManagerRepository(), _userManagerMock.Object);
        }

        [Fact]
        public async Task GetUser_ShouldReturnOk_WhenUserExists()
        {
            // Arrange
            var user = new IdentityUser { Id = "1", UserName = "user1", Email = "user1@example.com" };
            _userManagerMock.Setup(m => m.FindByEmailAsync("user1@example.com")).ReturnsAsync(user);

            // Act
            var result = await _controller.GetUser("user1@example.com");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }





        [Fact]
        public async Task AddUser_ShouldReturnOk_WhenUserAddedSuccessfully()
        {
            // Arrange
            var userModel = new User { Email = "user@example.com", UserName = "user", Phone = "1234567890", Password = "password", Roles = new string[] { "User" } };
            _userManagerMock.Setup(m => m.FindByEmailAsync("user@example.com")).ReturnsAsync((IdentityUser)null);
            _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), "password")).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(m => m.AddToRolesAsync(It.IsAny<IdentityUser>(), It.IsAny<IEnumerable<string>>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.addUser(userModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User Added Successfully", okResult.Value);
        }


        [Fact]
        public async Task UpdateUser_ShouldReturnOk_WhenUserUpdatedSuccessfully()
        {
            // Arrange
            var userModel = new User { Email = "user@example.com", UserName = "user", Phone = "1234567890" };
            var existingUser = new IdentityUser { Id = "1", UserName = "user", Email = "user@example.com" };
            _userManagerMock.Setup(m => m.FindByEmailAsync("user@example.com")).ReturnsAsync(existingUser);
            _userManagerMock.Setup(m => m.UpdateAsync(existingUser)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.UpdateUser(userModel, "user@example.com");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

 

        [Fact]
        public async Task DeleteUser_ShouldReturnOk_WhenUserDeletedSuccessfully()
        {
            // Arrange
            var existingUser = new IdentityUser { Id = "1", UserName = "user", Email = "user@example.com" };
            _userManagerMock.Setup(m => m.FindByEmailAsync("user@example.com")).ReturnsAsync(existingUser);
            _userManagerMock.Setup(m => m.DeleteAsync(existingUser)).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteUser("user@example.com");

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

       
    }

}
