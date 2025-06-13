using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.DTOs;
using WPF.Repositories;

namespace WPF.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly string _token;

        public CategoryService(ICategoryRepository repo, string token)
        {
            _repo = repo;
            _token = token;
        }

        public Task<IEnumerable<CategoryDto>> GetAllAsync() =>
            _repo.GetAllAsync(_token);

        public async Task<CategoryDto> CreateAsync(string name)
        {
            var dto = new CategoryDto { Name = name };
            return await _repo.CreateAsync(_token, dto);
        }

        public Task UpdateAsync(CategoryDto category) =>
            _repo.UpdateAsync(_token, category);

        public Task DeleteAsync(int id) =>
            _repo.DeleteAsync(_token, id);
    }
}
