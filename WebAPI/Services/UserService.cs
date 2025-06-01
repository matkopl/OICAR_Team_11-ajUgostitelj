using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;
using WebAPI.Security;
using static QRCoder.PayloadGenerator;

namespace WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public UserService(IRepositoryFactory repositoryFactory, IMapper mapper, AppDbContext context)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> CreateUserAsync(CreateUserDto createUserDto)
        {
            var userRepo = _repositoryFactory.GetRepository<User>();
            var existingUsers = await userRepo.GetAllAsync();

            if (existingUsers.Any(u => u.Username == createUserDto.Username)) 
            { 
                return false;
            }

            var salt = PasswordHashProvider.GetSalt();
            var hash = PasswordHashProvider.GetHash(createUserDto.Password, salt);

            var newUser = _mapper.Map<User>(createUserDto);
            newUser.Email = DataEncriptionProvider.Encrypt(newUser.Email);
            newUser.PwdSalt = salt;
            newUser.PwdHash = hash;
            newUser.RoleId = createUserDto.RoleId;

            await userRepo.AddAsync(newUser);
            await userRepo.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var userRepo = _repositoryFactory.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            userRepo.Remove(user);
            await userRepo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var userRepo = _repositoryFactory.GetRepository<User>();
            var users = await userRepo.GetAllAsync();
            return users.Select(user => new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = DataEncriptionProvider.Decrypt(user.Email),
                Role = user.Role != null ? user.Role.Name : (user.RoleId == 1 ? "Admin" : "User") 
            }).ToList();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var userRepo = _repositoryFactory.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Email = DataEncriptionProvider.Decrypt(user.Email);

            return userDto;
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                Console.WriteLine("User not found");
                return null;
            }

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Email = DataEncriptionProvider.Decrypt(user.Email);

            return userDto;
        }


        public async Task<bool> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            var userRepo = _repositoryFactory.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(updateUserDto.Id);

            if (user == null)
            {
                return false;
            }

            user.Username = updateUserDto.Username;
            user.Email = DataEncriptionProvider.Encrypt(updateUserDto.Email);
            user.RoleId = updateUserDto.RoleId;
            userRepo.Update(user);
            await userRepo.SaveChangesAsync();

            return true;
        }
    }
}
