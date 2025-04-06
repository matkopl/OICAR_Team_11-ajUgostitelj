using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
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
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(CreateUserDto createUserDto)
        {
            var success = await _userService.CreateUserAsync(createUserDto);

            if (!success)
            {
                return BadRequest($"User with username {createUserDto.Username} already exists");
            }

            return Ok($"Sucessfully created user: {createUserDto.Username}");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateUserDto)
        {
            updateUserDto.Id = id;

            var sucess = await _userService.UpdateUserAsync(updateUserDto);

            if (!sucess)
            {
                return NotFound();
            }

            return Ok($"User {updateUserDto.Username} with id {updateUserDto.Id} has been sucessfully updated");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var sucess = await _userService.DeleteUserAsync(id);

            if (!sucess)
            {
                return NotFound();
            }

            return Ok("User deleted sucessfully");
        }
    }
}
