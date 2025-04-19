using AutoMapper;
using Serilog;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;
using WebAPI.Security;

namespace WebAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public AuthService(IRepositoryFactory repositoryFactory, IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
        }

        public async Task<bool> ChangePasswordAsync(string username, ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userRepository = _repositoryFactory.GetRepository<User>();
                var user = (await userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Username == username);

                if (user == null)
                {
                    Log.Warning($"User {username} not found.");
                    return false;
                }

                if (!PasswordHashProvider.VerifyPassword(changePasswordDto.CurrentPassword, user.PwdHash, user.PwdSalt)) 
                {
                    Log.Warning($"Incorrect current password for user {username}.");
                    return false;
                }

                var salt = PasswordHashProvider.GetSalt();
                var hash = PasswordHashProvider.GetHash(changePasswordDto.NewPassword, salt);

                user.PwdSalt = salt;
                user.PwdHash = hash;

                userRepository.Update(user);
                await userRepository.SaveChangesAsync();
                Log.Information($"Password changed for user {username}");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred for user {username}.");
                throw;
            }
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var userRepository = _repositoryFactory.GetRepository<User>();

                var user = (await userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Username == loginDto.Username);

                if(user == null || !PasswordHashProvider.VerifyPassword(loginDto.Password, user.PwdHash, user.PwdSalt))
                {
                    Log.Warning($"Login failed. Username or password is incorrect. Please try again. {loginDto.Username}");
                    return null;
                }

                var token = JwtTokenProvider.CreateToken(
                    secureKey: "12345678901234567890123456789012",
                    expirationMinutes: 120,
                    user: user
                );
                Log.Information($"Login: User {loginDto.Username} logged in successfully.");
                return token;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Login: Error occurred for user {loginDto.Username}.");
                throw;
            }
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                var userRepository = _repositoryFactory.GetRepository<User>();
                var existingUser = (await userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Username == registerDto.Username);
                var username = registerDto.Username;
                
                if (existingUser != null)
                {
                    Log.Warning($"Username {username} already taken.");
                    return false;
                }

                var salt = PasswordHashProvider.GetSalt();
                var hash = PasswordHashProvider.GetHash(registerDto.Password, salt);

                var newUser = _mapper.Map<User>(registerDto);
                newUser.PwdSalt = salt;
                newUser.PwdHash = hash;
                newUser.RoleId = 2;

                await userRepository.AddAsync(newUser);
                await userRepository.SaveChangesAsync();
                Log.Information($"User {username} registered successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error occurred while registering user {registerDto.Username}.");
                throw;
            }
        }
    }
}
