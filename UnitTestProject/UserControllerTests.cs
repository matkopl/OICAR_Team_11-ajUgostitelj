using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;
using WebAPI.DTOs;
using WebAPI.Services;

namespace UnitTestProject
{
    public class UserControllerTests
    {
        [Fact]
        public async Task GetAllUsers_Returns_Ok()
        {
            var fakeService = A.Fake<IUserService>();
            var users = new List<UserDto> { new UserDto { Id = 1, Username = "john" } };

            A.CallTo(() => fakeService.GetAllUsersAsync()).Returns(users);

            var controller = new UserController(fakeService);
            var result = await controller.GetAllUsers();

            var ok = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsAssignableFrom<IEnumerable<UserDto>>(ok.Value);
            Assert.Single(data);
        }

        [Fact]
        public async Task GetAllUsers_Returns_BadRequest_On_Exception()
        {
            var fakeService = A.Fake<IUserService>();
            A.CallTo(() => fakeService.GetAllUsersAsync()).Throws(new Exception("DB error"));

            var controller = new UserController(fakeService);
            var result = await controller.GetAllUsers();

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Error fetching all tables, please see error log!", bad.Value);
        }

        [Fact]
        public async Task GetUserById_Returns_Ok_When_Found()
        {
            var fakeService = A.Fake<IUserService>();
            var user = new UserDto { Id = 1, Username = "jane" };

            A.CallTo(() => fakeService.GetUserByIdAsync(1)).Returns(user);

            var controller = new UserController(fakeService);
            var result = await controller.GetUserById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, ok.Value);
        }

        [Fact]
        public async Task GetUserById_Returns_NotFound_When_Null()
        {
            var fakeService = A.Fake<IUserService>();
            A.CallTo(() => fakeService.GetUserByIdAsync(1)).Returns((UserDto?)null);

            var controller = new UserController(fakeService);
            var result = await controller.GetUserById(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetUserById_Returns_BadRequest_On_Exception()
        {
            var fakeService = A.Fake<IUserService>();
            A.CallTo(() => fakeService.GetUserByIdAsync(1)).Throws(new Exception("DB error"));

            var controller = new UserController(fakeService);
            var result = await controller.GetUserById(1);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Error fetching user with ID", bad.Value!.ToString());
        }

        [Fact]
        public async Task GetUserByUsername_Returns_Ok_When_Found()
        {
            var fakeService = A.Fake<IUserService>();
            var user = new UserDto { Id = 1, Username = "admin" };

            A.CallTo(() => fakeService.GetUserByUsernameAsync("admin")).Returns(user);

            var controller = new UserController(fakeService);
            var result = await controller.GetUserByUsernameAsync("admin");

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, ok.Value);
        }

        [Fact]
        public async Task GetUserByUsername_Returns_NotFound_When_Null()
        {
            var fakeService = A.Fake<IUserService>();
            A.CallTo(() => fakeService.GetUserByUsernameAsync("ghost")).Returns((UserDto?)null);

            var controller = new UserController(fakeService);
            var result = await controller.GetUserByUsernameAsync("ghost");

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFound.Value);
        }

        [Fact]
        public async Task CreateUser_Returns_Ok_When_Success()
        {
            var fakeService = A.Fake<IUserService>();
            var dto = new CreateUserDto { Username = "newuser" };

            A.CallTo(() => fakeService.CreateUserAsync(dto)).Returns(true);

            var controller = new UserController(fakeService);
            var result = await controller.CreateUser(dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Successfully created user: newuser", ok.Value);
        }

        [Fact]
        public async Task CreateUser_Returns_BadRequest_When_UserExists()
        {
            var fakeService = A.Fake<IUserService>();
            var dto = new CreateUserDto { Username = "duplicate" };

            A.CallTo(() => fakeService.CreateUserAsync(dto)).Returns(false);

            var controller = new UserController(fakeService);
            var result = await controller.CreateUser(dto);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User with username duplicate already exists", bad.Value);
        }

        [Fact]
        public async Task UpdateUser_Returns_Ok_When_Success()
        {
            var fakeService = A.Fake<IUserService>();
            var dto = new UpdateUserDto { Username = "editme" };

            A.CallTo(() => fakeService.UpdateUserAsync(A<UpdateUserDto>.That.Matches(u => u.Id == 1 && u.Username == "editme")))
                .Returns(true);

            var controller = new UserController(fakeService);
            var result = await controller.UpdateUser(1, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("successfully updated", ok.Value!.ToString());
        }

        [Fact]
        public async Task UpdateUser_Returns_NotFound_When_Fails()
        {
            var fakeService = A.Fake<IUserService>();
            var dto = new UpdateUserDto { Username = "editfail" };

            A.CallTo(() => fakeService.UpdateUserAsync(A<UpdateUserDto>._)).Returns(false);

            var controller = new UserController(fakeService);
            var result = await controller.UpdateUser(1, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteUser_Returns_Ok_When_Success()
        {
            var fakeService = A.Fake<IUserService>();
            A.CallTo(() => fakeService.DeleteUserAsync(1)).Returns(true);

            var controller = new UserController(fakeService);
            var result = await controller.DeleteUser(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User deleted successfully", ok.Value);
        }

        [Fact]
        public async Task DeleteUser_Returns_NotFound_When_Fails()
        {
            var fakeService = A.Fake<IUserService>();
            A.CallTo(() => fakeService.DeleteUserAsync(1)).Returns(false);

            var controller = new UserController(fakeService);
            var result = await controller.DeleteUser(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
