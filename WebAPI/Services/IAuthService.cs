using WebAPI.DTOs;

namespace WebAPI.Services
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<bool> ChangePasswordAsync(string username, ChangePasswordDto changePasswordDto);
    }
}
