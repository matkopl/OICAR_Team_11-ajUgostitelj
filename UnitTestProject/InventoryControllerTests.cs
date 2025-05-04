using FakeItEasy;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;
using WebAPI.DTOs;
using WebAPI.Services;

namespace UnitTestProject
{
    public class InventoryControllerTests
    {
        [Fact]
        public async Task GetAll_Returns_Ok()
        {
            // Arrange
            var fakeService = A.Fake<IInventoryService>();
            var items = new List<InventoryDto> { new InventoryDto { Id = 1, ProductId = 10, Quantity = 5 } };
            A.CallTo(() => fakeService.GetAllInventoryItemsAsync()).Returns(items);

            var controller = new InventoryController(fakeService);

            // Act
            var result = await controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<InventoryDto>>(okResult.Value);
            Assert.Single(data);
        }

        [Fact]
        public async Task GetById_Returns_Ok_When_Found()
        {
            var fakeService = A.Fake<IInventoryService>();
            var dto = new InventoryDto { Id = 1, ProductId = 10, Quantity = 5 };
            A.CallTo(() => fakeService.GetInventoryItemByIdAsync(1)).Returns(dto);

            var controller = new InventoryController(fakeService);
            var result = await controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dto, ok.Value);
        }


        [Fact]
        public async Task GetByProductId_Returns_Ok_When_Found()
        {
            var fakeService = A.Fake<IInventoryService>();
            var dto = new InventoryDto { Id = 2, ProductId = 15, Quantity = 50 };
            A.CallTo(() => fakeService.GetInventoryByProductIdAsync(15)).Returns(dto);

            var controller = new InventoryController(fakeService);
            var result = await controller.GetByProductId(15);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task GetByProductId_Returns_NotFound_When_Null()
        {
            var fakeService = A.Fake<IInventoryService>();
            A.CallTo(() => fakeService.GetInventoryByProductIdAsync(99)).Returns((InventoryDto?)null);

            var controller = new InventoryController(fakeService);
            var result = await controller.GetByProductId(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_Returns_CreatedAt_When_Successful()
        {
            var fakeService = A.Fake<IInventoryService>();
            var input = new InventoryDto { ProductId = 10, Quantity = 20 };
            var created = new InventoryDto { Id = 1, ProductId = 10, Quantity = 20 };

            A.CallTo(() => fakeService.CreateInventoryItemAsync(input)).Returns(created);

            var controller = new InventoryController(fakeService);
            var result = await controller.Create(input);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created, createdResult.Value);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_On_Exception()
        {
            var fakeService = A.Fake<IInventoryService>();
            var input = new InventoryDto { ProductId = 10, Quantity = 20 };

            A.CallTo(() => fakeService.CreateInventoryItemAsync(input))
                .Throws(new InvalidOperationException("Something went wrong"));

            var controller = new InventoryController(fakeService);
            var result = await controller.Create(input);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Something went wrong", badRequest.Value);
        }

        [Fact]
        public async Task Update_Returns_NotFound_On_KeyNotFound()
        {
            var fakeService = A.Fake<IInventoryService>();
            var dto = new InventoryDto { Id = 1, ProductId = 10, Quantity = 50 };

            A.CallTo(() => fakeService.UpdateInventoryItemAsync(1, dto))
                .Throws(new KeyNotFoundException());

            var controller = new InventoryController(fakeService);
            var result = await controller.Update(1, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_NoContent_When_Successful()
        {
            var fakeService = A.Fake<IInventoryService>();
            A.CallTo(() => fakeService.DeleteInventoryItemAsync(1)).Returns(Task.CompletedTask);

            var controller = new InventoryController(fakeService);
            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
