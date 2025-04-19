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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get_all")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                Log.Information("All users fetched successfully!");
                return Ok(users);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest("Error fetching all tables, please see error log!");
            }
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    Log.Error($"User with ID {id} not found");
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, $"Error fetching user with ID {id}");
                return BadRequest($"Error fetching user with ID {id}, please see error log!");
            }
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserByUsernameAsync(string username)
        {
            var userDetails = await _userService.GetUserByUsernameAsync(username);

            if (userDetails == null)
            {
                return NotFound("User not found");
            }

            return Ok(userDetails);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            try
            {
                var success = await _userService.CreateUserAsync(createUserDto);

                if (!success)
                {
                    Log.Error($"User with username {createUserDto.Username} already exists");
                    return BadRequest($"User with username {createUserDto.Username} already exists");
                }

                return Ok($"Successfully created user: {createUserDto.Username}");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            try
            {
                updateUserDto.Id = id;

                var sucess = await _userService.UpdateUserAsync(updateUserDto);

                if (!sucess)
                {
                    Log.Error($"User {updateUserDto.Username} update failed!");
                    return NotFound();
                }
                Log.Information("Updating user successfully!");
                return Ok($"User {updateUserDto.Username} with id {updateUserDto.Id} has been successfully updated");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var sucess = await _userService.DeleteUserAsync(id);

                if (!sucess)
                {
                    Log.Error($"User delete failed!");
                    return NotFound();
                }

                return Ok("User deleted successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }
    }
}
