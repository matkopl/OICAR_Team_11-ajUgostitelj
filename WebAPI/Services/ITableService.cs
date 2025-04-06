using WebAPI.DTOs;

namespace WebAPI.Services
{
    public interface ITableService
    {
        Task<IEnumerable<TableDto>> GetAllTablesAsync();
        Task<TableDto?> GetTableByIdAsync(int id);
        Task<TableDto> CreateTableAsync(TableDto tableDto);
        Task UpdateTableAsync(int id, TableDto tableDto);
        Task DeleteTableAsync(int id);
        Task<bool> TableExistsAsync(int id);
        Task<(List<TableDto> Tables, int TotalCount)> GetTablesPagedAsync(TableQueryDto query);
        List<string> GetAvailableSortColumns();
    }
}
