using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(string token);
        Task<UserDto?> GetUserByUsernameAsync(string username, string token);
        Task<bool> CreateUserAsync(string token, CreateUserDto createUserDto);
        Task<bool> UpdateUserAsync(string token, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(string token, int userId);
        Task<bool> AnonymizeUserAsync(string token, int userId);
    }
}
