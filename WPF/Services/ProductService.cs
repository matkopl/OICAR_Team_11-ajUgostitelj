using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.DTOs;
using WPF.Repositories;

namespace WPF.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;
        private readonly string _token;

        public ProductService(IProductRepository repo, string token)
        {
            _repo = repo;
            _token = token;
        }

        public Task<IEnumerable<ProductDto>> GetAllProductsAsync() =>
            _repo.GetAllAsync(_token);

        public Task<ProductDto> CreateProductAsync(ProductDto product) =>
            _repo.CreateAsync(_token, product);

        public Task UpdateProductAsync(int id, ProductDto product) =>
            _repo.UpdateAsync(_token, id, product);

        public Task DeleteProductAsync(int id) =>
            _repo.DeleteAsync(_token, id);
    }
}
