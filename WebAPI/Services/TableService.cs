// TableService.cs
using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using Microsoft.AspNetCore.Http.HttpResults;

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
            try
            {
                var tables = await repo.GetAllAsync();
                return _mapper.Map<IEnumerable<TableDto>>(tables);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error fetching all tables in the service layer");
                throw;
            }
        }

        public async Task<TableDto?> GetTableByIdAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Table>();
            try
            {
                var table = await repo.GetByIdAsync(id);
                return _mapper.Map<TableDto>(table);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error fetching table with ID {id} in the service layer");
                throw;
            }
        }

        public async Task<TableDto> CreateTableAsync(TableDto tableDto)
        {
            var repo = _repositoryFactory.GetRepository<Table>();
            try
            {
                var table = _mapper.Map<Table>(tableDto);

                await repo.AddAsync(table);
                await repo.SaveChangesAsync();

                return _mapper.Map<TableDto>(table);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating table in the service layer");
                throw;
            }
        }

        public async Task UpdateTableAsync(int id, TableDto tableDto)
        {
            var repo = _repositoryFactory.GetRepository<Table>();
            var existingTable = await repo.GetByIdAsync(id);

            if (existingTable == null)
            {
                Log.Error($"Table with ID {id} not found for update in the service layer");
                throw new KeyNotFoundException("Table not found");
            }

            // Osiguravamo da ID u URL-u i DTO-u matchaju
            tableDto.Id = id;
            _mapper.Map(tableDto, existingTable);
            repo.Update(existingTable);

            try
            {
                await repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error updating table with ID {id} in the service layer");
                throw;
            }
        }

        public async Task DeleteTableAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Table>();
            var table = await repo.GetByIdAsync(id);

            if (table == null)
            {
                Log.Error($"Table with ID {id} not found for deleting it, error in the service layer");
                throw new KeyNotFoundException("Table not found");
            }

            repo.Remove(table);
            try
            {
                await repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error deleting table with ID {id} in the service layer");
                throw;
            }
        }

        public async Task<bool> TableExistsAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Table>();
            try
            {
                return await repo.GetByIdAsync(id) != null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Table doesn't exists table with ID: {id}");
                throw;
            }
        }
    }
}
