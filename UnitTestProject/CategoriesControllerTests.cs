using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;
using WebAPI.DTOs;
using WebAPI.Services;

namespace UnitTestProject
{
    public class CategoriesControllerTests
    {

        [Fact]
        public async Task GetAll_Returns_Ok_With_List()
        {
            // Arrange
            var fakeService = A.Fake<ICategoryService>();
            var expected = new List<CategoryDto>
        {
            new CategoryDto { Id = 1, Name = "Starters" },
            new CategoryDto { Id = 2, Name = "Desserts" }
        };

            A.CallTo(() => fakeService.GetAllCategoriesAsync())
                .Returns(Task.FromResult<IEnumerable<CategoryDto>>(expected));

            var controller = new CategoriesController(fakeService);

            // Act
            var result = await controller.GetAll();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var categories = Assert.IsAssignableFrom<IEnumerable<CategoryDto>>(ok.Value);
            Assert.Equal(2, categories.Count());
        }

        [Fact]
        public async Task GetById_Returns_Ok_When_Found()
        {
            // Arrange
            var fakeService = A.Fake<ICategoryService>();
            var id = 1;
            var category = new CategoryDto { Id = id, Name = "Drinks" };

            A.CallTo(() => fakeService.GetCategoryByIdAsync(id))
                .Returns(Task.FromResult(category));

            var controller = new CategoriesController(fakeService);

            // Act
            var result = await controller.GetById(id);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<CategoryDto>(ok.Value);
            Assert.Equal(id, returned.Id);
        }

        [Fact]
        public async Task Create_Returns_CreatedAt_When_Valid()
        {
            // Arrange
            var fakeService = A.Fake<ICategoryService>();
            var input = new CategoryDto { Name = "New Category" };
            var created = new CategoryDto { Id = 1, Name = "New Category" };

            A.CallTo(() => fakeService.CreateCategoryAsync(input))
                .Returns(Task.FromResult(created));

            var controller = new CategoriesController(fakeService);

            // Act
            var result = await controller.Create(input);

            // Assert
            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returned = Assert.IsType<CategoryDto>(createdAt.Value);
            Assert.Equal(created.Id, returned.Id);
        }

        [Fact]
        public async Task Update_Returns_NoContent_When_Successful()
        {
            // Arrange
            var fakeService = A.Fake<ICategoryService>();
            var id = 1;
            var dto = new CategoryDto { Id = id, Name = "Updated" };

            A.CallTo(() => fakeService.UpdateCategoryAsync(id, dto))
                .Returns(Task.CompletedTask);

            var controller = new CategoriesController(fakeService);

            // Act
            var result = await controller.Update(id, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_NoContent_When_Successful()
        {
            // Arrange
            var fakeService = A.Fake<ICategoryService>();
            var id = 1;

            A.CallTo(() => fakeService.DeleteCategoryAsync(id))
                .Returns(Task.CompletedTask);

            var controller = new CategoriesController(fakeService);

            // Act
            var result = await controller.Delete(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
