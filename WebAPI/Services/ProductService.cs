using AutoMapper;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;

namespace WebAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IMapper _mapper;

        public ProductService(IRepositoryFactory repositoryFactory, IMapper mapper, AppDbContext context)
        {
            _repositoryFactory = repositoryFactory;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var repo = _repositoryFactory.GetRepository<Product>();
            var products = await repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Product>();
            var product = await repo.GetByIdAsync(id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
        {
            var repo = _repositoryFactory.GetRepository<Product>();
            var product = _mapper.Map<Product>(productDto);

            await repo.AddAsync(product);
            await repo.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task UpdateProductAsync(int id, ProductDto productDto)
        {
            var repo = _repositoryFactory.GetRepository<Product>();
            var existingProduct = await repo.GetByIdAsync(id);

            if (existingProduct == null)
                throw new KeyNotFoundException("Product not found");

            _mapper.Map(productDto, existingProduct);
            repo.Update(existingProduct);
            await repo.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var repo = _repositoryFactory.GetRepository<Product>();
            var product = await repo.GetByIdAsync(id);

            if (product == null)
                throw new KeyNotFoundException("Product not found");

            repo.Remove(product);
            await repo.SaveChangesAsync();
        }

        // Nova implementacija metode za filtriranje proizvoda prema kategoriji
        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var repo = _repositoryFactory.GetRepository<Product>();
            var products = await repo.FindAsync(p => p.CategoryId == categoryId);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}
