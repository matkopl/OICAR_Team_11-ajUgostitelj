using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.DTOs;

namespace WPF.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<CategoryDto> CreateAsync(string name);
        Task UpdateAsync(CategoryDto category);
        Task DeleteAsync(int id);
    }
}
