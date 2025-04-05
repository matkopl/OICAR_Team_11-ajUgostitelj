using AutoMapper;
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
            var userRepository = _repositoryFactory.GetRepository<User>();
            var user = (await userRepository.GetAllAsync())
                .FirstOrDefault(u => u.Username == username);

            if (user == null || !PasswordHashProvider.VerifyPassword(changePasswordDto.CurrentPassword, user.PwdHash, user.PwdSalt))
            {
                return false;
            }

            var salt = PasswordHashProvider.GetSalt();
            var hash = PasswordHashProvider.GetHash(changePasswordDto.NewPassword, salt);

            user.PwdSalt = salt;
            user.PwdHash = hash;

            userRepository.Update(user);
            await userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var userRepository = _repositoryFactory.GetRepository<User>();

            var user = (await userRepository.GetAllAsync())
                .FirstOrDefault(u => u.Username == loginDto.Username);

            if (user == null || !PasswordHashProvider.VerifyPassword(loginDto.Password, user.PwdHash, user.PwdSalt))
            {
                return "Login failed. Username or password is incorrect. Please try again.";
            }

            var token = JwtTokenProvider.CreateToken(
                secureKey: "12345678901234567890123456789012",
                expirationMinutes: 120,
                user: user
            );

            return token;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var userRepository = _repositoryFactory.GetRepository<User>();
            var existingUser = (await userRepository.GetAllAsync())
                .FirstOrDefault(u => u.Username == registerDto.Username);

            if (existingUser != null)
            {
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
            return true;
        }
    }
}
