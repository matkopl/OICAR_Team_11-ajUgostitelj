using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Controllers;
using WebAPI.DTOs;
using WebAPI.Services;


namespace UnitTestProject
{
    public class AuthControllerTests
    {
        [Fact]
        public async Task Register_Returns_Ok_When_Successful()
        {
            
            var fakeAuthService = A.Fake<IAuthService>();
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Password = "password1", 
                Email = "test@example.com"
            };

            A.CallTo(() => fakeAuthService.RegisterAsync(registerDto))
                .Returns(Task.FromResult(true));

            var controller = new AuthController(fakeAuthService);

            
            var result = await controller.Register(registerDto);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = Assert.IsType<string>(okResult.Value);
            Assert.Contains("successfully registered", message);
        }

        [Fact]
        public async Task Register_Returns_BadRequest_When_Username_Already_Taken()
        {
            
            var fakeAuthService = A.Fake<IAuthService>();
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Password = "password1",
                Email = "test@example.com"
            };

            A.CallTo(() => fakeAuthService.RegisterAsync(registerDto))
                .Returns(Task.FromResult(false));

            var controller = new AuthController(fakeAuthService);

            
            var result = await controller.Register(registerDto);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Username already taken!", badRequestResult.Value);
        }


        [Fact]
        public async Task Login_Returns_Unauthorized_When_Login_Fails()
        {
            
            var fakeAuthService = A.Fake<IAuthService>();
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "wrongpass"
            };

            A.CallTo(() => fakeAuthService.LoginAsync(loginDto))
                .Returns(Task.FromResult<string?>(null));

            var controller = new AuthController(fakeAuthService);

            
            var result = await controller.Login(loginDto);

            
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task ChangePassword_Returns_Ok_When_Successful()
        {
            
            var fakeAuthService = A.Fake<IAuthService>();
            var changePasswordDto = new ChangePasswordDto
            {
                CurrentPassword = "oldpassword",
                NewPassword = "newpassword1" 
            };

            A.CallTo(() => fakeAuthService.ChangePasswordAsync("testuser", changePasswordDto))
                .Returns(Task.FromResult(true));

            var controller = new AuthController(fakeAuthService);

            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            
            var result = await controller.ChangePassword(changePasswordDto);

            
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Password has been changed successfully", okResult.Value);
        }

        [Fact]
        public async Task ChangePassword_Returns_Unauthorized_When_No_Username_Found()
        {
            
            var fakeAuthService = A.Fake<IAuthService>();
            var changePasswordDto = new ChangePasswordDto
            {
                CurrentPassword = "oldpassword",
                NewPassword = "newpassword1"
            };

            var controller = new AuthController(fakeAuthService);

            
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

            
            var result = await controller.ChangePassword(changePasswordDto);

            
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task ChangePassword_Returns_BadRequest_When_Current_Password_Incorrect()
        {
            
            var fakeAuthService = A.Fake<IAuthService>();
            var changePasswordDto = new ChangePasswordDto
            {
                CurrentPassword = "oldpassword",
                NewPassword = "newpassword1"
            };

            A.CallTo(() => fakeAuthService.ChangePasswordAsync("testuser", changePasswordDto))
                .Returns(Task.FromResult(false));

            var controller = new AuthController(fakeAuthService);

            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            
            var result = await controller.ChangePassword(changePasswordDto);

            
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Current password is incorrect", badRequestResult.Value);
        }
    }
}
