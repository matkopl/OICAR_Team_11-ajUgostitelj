using WebAPI.DTOs;

namespace WebAPI.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<bool> CreateUserAsync(CreateUserDto createUserDto);
        Task<bool> UpdateUserAsync(UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
    }
}
