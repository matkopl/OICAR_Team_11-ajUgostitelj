using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Repositories
{
    public interface ITableRepository
    {
        Task<IEnumerable<TableDto>> GetAllAsync(string token);
        Task<TableDto> CreateAsync(string token, TableDto table);
        Task DeleteAsync(string token, int id);
    }
}
