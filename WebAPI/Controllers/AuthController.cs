using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var success = await _authService.RegisterAsync(registerDto);

            if (!success)
            {
                return BadRequest("Username already taken");
            }

            return Ok($"User {registerDto.Username} has been successfully registered");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(new { Token = token });
        }

        [Authorize]
        [HttpPost("change_password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var success = await _authService.ChangePasswordAsync(username, changePasswordDto);

            if (!success)
            {
                return BadRequest("Current password is incorrect");
            }
            return Ok("Password has been changed successfully");
        }
    }
}
