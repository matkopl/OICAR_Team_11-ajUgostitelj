using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public RoleService(IRepositoryFactory repositoryFactory, IMapper mapper, AppDbContext context)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            return await _context.Roles
                .Include(r => r.Users)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    UserCount = r.Users.Count
                })
                .ToListAsync();
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return null;

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                UserCount = role.Users.Count
            };
        }

        public async Task<RoleDto> CreateRoleAsync(RoleDto roleDto)
        {
            // Provjera da li role s istim imenom već postoji
            var roleExists = await _context.Roles.AnyAsync(r => r.Name == roleDto.Name);
            if (roleExists)
                throw new InvalidOperationException("Role with this name already exists");

            var role = new Role
            {
                Name = roleDto.Name.Trim()
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                UserCount = 0
            };
        }

        public async Task UpdateRoleAsync(int id, RoleDto roleDto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                throw new KeyNotFoundException("Role not found");

            // Provjera da li role s istim imenom već postoji (osim trenutne)
            var roleWithSameName = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleDto.Name && r.Id != id);

            if (roleWithSameName != null)
                throw new InvalidOperationException("Another role with this name already exists");

            role.Name = roleDto.Name.Trim();
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                throw new KeyNotFoundException("Role not found");

            if (role.Users.Any())
                throw new InvalidOperationException("Cannot delete role with assigned users");

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}