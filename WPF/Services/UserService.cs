using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;
using WPF.Repositories;

namespace WPF.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }
        public async Task<bool> CreateUserAsync(string username, CreateUserDto createUserDto)
        {
            return await _repository.CreateUserAsync(username, createUserDto);
        }

        public async Task<bool> DeleteUserAsync(string token, int userId)
        {
            return await _repository.DeleteUserAsync(token, userId);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(string token)
        {
            return await _repository.GetAllUsersAsync(token);
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username, string token)
        {
            return await _repository.GetUserByUsernameAsync(username, token);
        }

        public async Task<bool> UpdateUserAsync(string token, UpdateUserDto updateUserDto)
        {
            return await _repository.UpdateUserAsync(token, updateUserDto);
        }

        public async Task<bool> AnonymizeUserAsync(string token, int userId)
        {
            return await _repository.AnonymizeUserAsync(token, userId);
        }
    }
}
