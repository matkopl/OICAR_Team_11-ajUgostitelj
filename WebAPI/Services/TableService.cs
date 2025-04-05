// TableService.cs
using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Services
{
    public class TableService : ITableService
    {
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public TableService(IRepositoryFactory repositoryFactory, IMapper mapper)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TableDto>> GetAllTablesAsync()
        {
            var repo = _repositoryFactory.GetRepository<Table>();
            var tables = await repo.GetAllAsync();
            return _mapper.Map<IEnumerable<TableDto>>(tables);
        }

        public async Task<TableDto?> GetTableByIdAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Table>();
            var table = await repo.GetByIdAsync(id);
            return _mapper.Map<TableDto>(table);
        }

        public async Task<TableDto> CreateTableAsync(TableDto tableDto)
        {
            var repo = _repositoryFactory.GetRepository<Table>();
            var table = _mapper.Map<Table>(tableDto);

            await repo.AddAsync(table);
            await repo.SaveChangesAsync();

            return _mapper.Map<TableDto>(table);
        }

        public async Task UpdateTableAsync(int id, TableDto tableDto)
        {
            var repo = _repositoryFactory.GetRepository<Table>();
            var existingTable = await repo.GetByIdAsync(id);

            if (existingTable == null)
                throw new KeyNotFoundException("Table not found");

            // Osiguravamo da ID u URL-u i DTO-u matchaju
            tableDto.Id = id;
            _mapper.Map(tableDto, existingTable);

            repo.Update(existingTable);
            await repo.SaveChangesAsync();
        }

        public async Task DeleteTableAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Table>();
            var table = await repo.GetByIdAsync(id);

            if (table == null)
                throw new KeyNotFoundException("Table not found");

            repo.Remove(table);
            await repo.SaveChangesAsync();
        }

        public async Task<bool> TableExistsAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Table>();
            return await repo.GetByIdAsync(id) != null;
        }
    }
}
