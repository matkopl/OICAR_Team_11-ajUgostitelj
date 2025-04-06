using WebAPI.DTOs;

namespace WebAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<ProductDto> CreateProductAsync(ProductDto productDto);
        Task UpdateProductAsync(int id, ProductDto productDto);
        Task DeleteProductAsync(int id);

        // Nova metoda za filtriranje proizvoda prema kategoriji
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId);
    }
}
