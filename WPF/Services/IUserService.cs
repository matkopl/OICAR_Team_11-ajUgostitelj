using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(string token);
        Task<UserDto?> GetUserByUsernameAsync(string username, string token);
        Task<bool> CreateUserAsync(string username, CreateUserDto createUserDto);
        Task<bool> UpdateUserAsync(string token, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(string token, int userId);
        Task<bool> AnonymizeUserAsync(string token, int userId);
    }
}
