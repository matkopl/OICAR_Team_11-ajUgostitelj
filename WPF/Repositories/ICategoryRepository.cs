using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync(string token);
        Task<CategoryDto> CreateAsync(string token, CategoryDto category);
        Task UpdateAsync(string token, CategoryDto category);
        Task DeleteAsync(string token, int categoryId);
    }
}
