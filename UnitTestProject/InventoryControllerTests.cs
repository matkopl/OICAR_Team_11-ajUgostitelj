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
        public async Task GetAll_Returns_Ok_With_Data()
        {
            var fakeService = A.Fake<IInventoryService>();
            var items = new List<InventoryDto> { new InventoryDto { Id = 1, ProductId = 10, Quantity = 5 } };
            A.CallTo(() => fakeService.GetAllInventoriesAsync()).Returns(items);

            var controller = new InventoryController(fakeService);

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<InventoryDto>>(okResult.Value);
            Assert.Single(data);
        }

        [Fact]
        public async Task GetAll_Returns_Ok_With_EmptyList()
        {
            var fakeService = A.Fake<IInventoryService>();
            A.CallTo(() => fakeService.GetAllInventoriesAsync()).Returns(new List<InventoryDto>());

            var controller = new InventoryController(fakeService);

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<InventoryDto>>(okResult.Value);
            Assert.Empty(data);
        }

        [Fact]
        public async Task GetAll_Returns_BadRequest_On_Exception()
        {
            var fakeService = A.Fake<IInventoryService>();
            A.CallTo(() => fakeService.GetAllInventoriesAsync()).Throws(new Exception("Database error"));

            var controller = new InventoryController(fakeService);

            var result = await controller.GetAll();

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Database error", badRequest.Value);
        }

        [Fact]
        public async Task AddProductToInventory_Returns_Ok_When_Successful()
        {
            var fakeService = A.Fake<IInventoryService>();
            var inventoryDto = new InventoryDto { ProductId = 10, Quantity = 5 };

            A.CallTo(() => fakeService.AddProductToInventoryAsync(inventoryDto)).Returns(true);

            var controller = new InventoryController(fakeService);

            var result = await controller.AddProductToInventory(inventoryDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Product added to inventory", okResult.Value);
        }

        [Fact]
        public async Task AddProductToInventory_Returns_BadRequest_When_ProductAlreadyExists()
        {
            var fakeService = A.Fake<IInventoryService>();
            var inventoryDto = new InventoryDto { ProductId = 10, Quantity = 5 };

            A.CallTo(() => fakeService.AddProductToInventoryAsync(inventoryDto)).Returns(false);

            var controller = new InventoryController(fakeService);

            var result = await controller.AddProductToInventory(inventoryDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("This product is already in inventory!", badRequestResult.Value);
        }

        [Fact]
        public async Task AddProductToInventory_Returns_BadRequest_On_Exception()
        {
            var fakeService = A.Fake<IInventoryService>();
            var inventoryDto = new InventoryDto { ProductId = 10, Quantity = 5 };

            A.CallTo(() => fakeService.AddProductToInventoryAsync(inventoryDto)).Throws(new Exception("Database error"));

            var controller = new InventoryController(fakeService);

            var result = await controller.AddProductToInventory(inventoryDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Database error", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateInventory_Returns_Ok_When_Successful()
        {
            var fakeService = A.Fake<IInventoryService>();
            var inventoryDto = new InventoryDto { Id = 1, ProductId = 10, Quantity = 10 };

            A.CallTo(() => fakeService.UpdateInventoryAsync(inventoryDto)).Returns(true);

            var controller = new InventoryController(fakeService);

            var result = await controller.UpdateInventory(1, inventoryDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Inventory updated successfully", okResult.Value);
        }

        [Fact]
        public async Task UpdateInventory_Returns_NotFound_When_Inventory_Does_Not_Exist()
        {
            var fakeService = A.Fake<IInventoryService>();
            var inventoryDto = new InventoryDto { Id = 99, ProductId = 20, Quantity = 5 };

            A.CallTo(() => fakeService.UpdateInventoryAsync(inventoryDto)).Returns(false);

            var controller = new InventoryController(fakeService);

            var result = await controller.UpdateInventory(99, inventoryDto);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Inventory not found", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateInventory_Returns_BadRequest_On_Exception()
        {
            var fakeService = A.Fake<IInventoryService>();
            var inventoryDto = new InventoryDto { Id = 1, ProductId = 10, Quantity = 5 };

            A.CallTo(() => fakeService.UpdateInventoryAsync(inventoryDto)).Throws(new Exception("Database error"));

            var controller = new InventoryController(fakeService);

            var result = await controller.UpdateInventory(1, inventoryDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Database error", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteInventory_Returns_Ok_When_Successful()
        {
            var fakeService = A.Fake<IInventoryService>();
            A.CallTo(() => fakeService.DeleteInventoryAsync(1)).Returns(true);

            var controller = new InventoryController(fakeService);

            var result = await controller.DeleteInventory(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Inventory deleted successfully", okResult.Value);
        }

        [Fact]
        public async Task DeleteInventory_Returns_NotFound_When_Inventory_Does_Not_Exist()
        {
            var fakeService = A.Fake<IInventoryService>();
            A.CallTo(() => fakeService.DeleteInventoryAsync(99)).Returns(false);

            var controller = new InventoryController(fakeService);

            var result = await controller.DeleteInventory(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task DeleteInventory_Returns_BadRequest_On_Exception()
        {
            var fakeService = A.Fake<IInventoryService>();

            A.CallTo(() => fakeService.DeleteInventoryAsync(1)).Throws(new Exception("Database error"));

            var controller = new InventoryController(fakeService);

            var result = await controller.DeleteInventory(1);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Database error", badRequestResult.Value);
        }
    }
}
