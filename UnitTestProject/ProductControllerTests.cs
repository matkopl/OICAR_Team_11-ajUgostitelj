using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Controllers;
using WebAPI.DTOs;
using WebAPI.Services;
using Xunit;

namespace UnitTestProject
{
    public class ProductControllerTests
    {
        [Fact]
        public async Task GetAll_Returns_Ok()
        {
            
            var fakeProductService = A.Fake<IProductService>();
            var expectedProducts = new List<ProductDto>
            {
                new ProductDto { Id = 1, Name = "Kava", Description = "Aroma kava", Price = 5.00m, CategoryId = 1 },
                new ProductDto { Id = 2, Name = "Čaj", Description = "Zeleni čaj", Price = 4.50m, CategoryId = 2 }
            };

            A.CallTo(() => fakeProductService.GetAllProductsAsync())
                .Returns(Task.FromResult<IEnumerable<ProductDto>>(expectedProducts));

            var controller = new ProductController(fakeProductService);

            
            var result = await controller.GetAll();

            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            Assert.Equal(expectedProducts, returnedProducts);
        }

        [Fact]
        public async Task GetById_Returns_Ok_When_Found()
        {
            
            int testProductId = 1;
            var expectedProduct = new ProductDto { Id = testProductId, Name = "Kava", Description = "Aroma kava", Price = 5.00m, CategoryId = 1 };

            var fakeProductService = A.Fake<IProductService>();
            A.CallTo(() => fakeProductService.GetProductByIdAsync(testProductId))
                .Returns(Task.FromResult<ProductDto?>(expectedProduct));

            var controller = new ProductController(fakeProductService);

           
            var result = await controller.GetById(testProductId);

            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
            Assert.Equal(expectedProduct.Id, returnedProduct.Id);
            Assert.Equal(expectedProduct.Name, returnedProduct.Name);
        }

        [Fact]
        public async Task GetById_Returns_NotFound_When_NotFound()
        {
            
            int testProductId = 1;
            var fakeProductService = A.Fake<IProductService>();
            A.CallTo(() => fakeProductService.GetProductByIdAsync(testProductId))
                .Returns(Task.FromResult<ProductDto?>(null));

            var controller = new ProductController(fakeProductService);

            
            var result = await controller.GetById(testProductId);

            
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_Returns_CreatedAtAction()
        {
            
            var fakeProductService = A.Fake<IProductService>();

            var inputProduct = new ProductDto
            {
                Id = 0,
                Name = "Kava",
                Description = "Aroma kava",
                Price = 5.00m,
                CategoryId = 1
            };

            var createdProduct = new ProductDto
            {
                Id = 1,
                Name = inputProduct.Name,
                Description = inputProduct.Description,
                Price = inputProduct.Price,
                CategoryId = inputProduct.CategoryId
            };

            A.CallTo(() => fakeProductService.CreateProductAsync(inputProduct))
                .Returns(Task.FromResult(createdProduct));

            var controller = new ProductController(fakeProductService);

            
            var result = await controller.Create(inputProduct);

            
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductDto>(createdAtResult.Value);
            Assert.Equal(createdProduct.Id, returnedProduct.Id);
            Assert.Equal(createdProduct.Name, returnedProduct.Name);
            Assert.Equal(createdProduct.Description, returnedProduct.Description);
            Assert.Equal(createdProduct.Price, returnedProduct.Price);
            Assert.Equal(createdProduct.CategoryId, returnedProduct.CategoryId);
        }

        [Fact]
        public async Task Update_Returns_NoContent_On_Success()
        {
            
            int productId = 1;
            var productDto = new ProductDto
            {
                Id = productId,
                Name = "Čaj",
                Description = "Zeleni čaj",
                Price = 4.50m,
                CategoryId = 2
            };

            var fakeProductService = A.Fake<IProductService>();
            A.CallTo(() => fakeProductService.UpdateProductAsync(productId, productDto))
                .Returns(Task.CompletedTask);

            var controller = new ProductController(fakeProductService);

            
            var result = await controller.Update(productId, productDto);

            
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_NoContent_On_Success()
        {
            
            int productId = 1;
            var fakeProductService = A.Fake<IProductService>();
            A.CallTo(() => fakeProductService.DeleteProductAsync(productId))
                .Returns(Task.CompletedTask);

            var controller = new ProductController(fakeProductService);

            
            var result = await controller.Delete(productId);

            
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetByCategory_Returns_Ok()
        {
            
            int categoryId = 1;
            var expectedProducts = new List<ProductDto>
            {
                new ProductDto { Id = 1, Name = "Kava", Description = "Aroma kava", Price = 5.00m, CategoryId = categoryId },
                new ProductDto { Id = 2, Name = "Espresso", Description = "Jaka kava", Price = 6.00m, CategoryId = categoryId }
            };

            var fakeProductService = A.Fake<IProductService>();
            A.CallTo(() => fakeProductService.GetProductsByCategoryAsync(categoryId))
                .Returns(Task.FromResult<IEnumerable<ProductDto>>(expectedProducts));

            var controller = new ProductController(fakeProductService);

            
            var result = await controller.GetByCategory(categoryId);

           
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
            Assert.Equal(expectedProducts, returnedProducts);
        }
    }
}
