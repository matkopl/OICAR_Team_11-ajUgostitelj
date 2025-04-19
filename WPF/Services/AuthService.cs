using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;
using WPF.Repositories;

namespace WPF.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<bool> ChangePasswordAsync(string token, ChangePasswordDto changePasswordDto)
        {
            return await _authRepository.ChangePasswordAsync(token, changePasswordDto);
        }

        public async Task<UserDto?> GetUserDetailsAsync(string username, string token)
        {
            return await _authRepository.GetUserDetailsAsync(username, token);
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            return await _authRepository.LoginAsync(username, password);
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            return await _authRepository.RegisterAsync(registerDto);
        }

       
    }
}
