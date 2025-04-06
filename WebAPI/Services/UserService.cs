﻿using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;
using WebAPI.Security;

namespace WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public UserService(IRepositoryFactory repositoryFactory, IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
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
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var userRepo = _repositoryFactory.GetRepository<User>();
            var user = await userRepo.GetByIdAsync(id);

            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserDto>(user);
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
            user.Email = updateUserDto.Email;

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                var salt = PasswordHashProvider.GetSalt();
                var hash = PasswordHashProvider.GetHash(updateUserDto.Password, salt);
                user.PwdSalt = salt;
                user.PwdHash = hash;
            }

            userRepo.Update(user);
            await userRepo.SaveChangesAsync();

            return true;
        }
    }
}
