using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
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
            try
            {
                Log.Information($"Attempting to register user: ", registerDto.Username);
                var success = await _authService.RegisterAsync(registerDto);

                if (!success)
                {
                    Log.Warning($"Registration failed: Username already taken ", registerDto.Username);
                    return BadRequest("Username already taken!");
                }
                Log.Information($"User {registerDto.Username} registered successfully");
                return Ok($"User {registerDto.Username} has been successfully registered");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error registering user ", registerDto.Username);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                Log.Information($"Logging in for user: {loginDto.Username}");
                var token = await _authService.LoginAsync(loginDto);

                if (token == null)
                {
                    Log.Error($"Login failed for user , {loginDto.Username}");
                    return Unauthorized();
                }
                Log.Information($"User {loginDto.Username} logged in successfully");
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error logging in user {loginDto.Username}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [Authorize]
        [HttpPost("change_password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var username = User.Identity?.Name;
            try
            {
               
                if (string.IsNullOrEmpty(username))
                {
                    Log.Error($"Username: {username} not found");
                    return Unauthorized();
                }
                
                Log.Information($"User {username} is attempting to change password");
                var success = await _authService.ChangePasswordAsync(username, changePasswordDto);

                if (!success)
                {
                    Log.Warning($"Change password failed for user {username}: current password is incorrect");
                    return BadRequest("Current password is incorrect");
                }
                
                Log.Information($"Password changed successfully for user {username}");
                return Ok("Password has been changed successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error changing password for user {username}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
