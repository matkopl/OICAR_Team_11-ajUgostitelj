using FakeItEasy;
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
    public class OrderItemsControllerTests
    {
        [Fact]
        public async Task GetAll_Returns_Ok()
        {
            var fakeService = A.Fake<IOrderItemService>();
            var expected = new List<OrderItemDto> { new OrderItemDto { Id = 1, OrderId = 10, ProductId = 5, Quantity = 2 } };

            A.CallTo(() => fakeService.GetAllOrderItemsAsync()).Returns(expected);

            var controller = new OrderItemsController(fakeService);
            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<OrderItemDto>>(ok.Value);
            Assert.Single(data);
        }

        [Fact]
        public async Task GetById_Returns_Ok_When_Found()
        {
            var fakeService = A.Fake<IOrderItemService>();
            var dto = new OrderItemDto { Id = 1, OrderId = 10 };

            A.CallTo(() => fakeService.GetOrderItemByIdAsync(1)).Returns(dto);

            var controller = new OrderItemsController(fakeService);
            var result = await controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task GetById_Returns_NotFound_When_Null()
        {
            var fakeService = A.Fake<IOrderItemService>();
            A.CallTo(() => fakeService.GetOrderItemByIdAsync(99)).Returns((OrderItemDto?)null);

            var controller = new OrderItemsController(fakeService);
            var result = await controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetByOrderId_Returns_Ok()
        {
            var fakeService = A.Fake<IOrderItemService>();
            var expected = new List<OrderItemDto>
        {
            new OrderItemDto { Id = 1, OrderId = 10, ProductId = 2, Quantity = 3 }
        };

            A.CallTo(() => fakeService.GetOrderItemsByOrderIdAsync(10)).Returns(expected);

            var controller = new OrderItemsController(fakeService);
            var result = await controller.GetByOrderId(10);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<OrderItemDto>>(ok.Value);
            Assert.Single(data);
        }

        [Fact]
        public async Task Create_Returns_CreatedAt_When_Successful()
        {
            var fakeService = A.Fake<IOrderItemService>();
            var input = new OrderItemDto { OrderId = 10, ProductId = 5, Quantity = 2 };
            var created = new OrderItemDto { Id = 1, OrderId = 10, ProductId = 5, Quantity = 2 };

            A.CallTo(() => fakeService.CreateOrderItemAsync(input)).Returns(created);

            var controller = new OrderItemsController(fakeService);
            var result = await controller.Create(input);

            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created.Id, ((OrderItemDto)createdAt.Value).Id);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_On_Exception()
        {
            var fakeService = A.Fake<IOrderItemService>();
            var input = new OrderItemDto { OrderId = 10, ProductId = 5, Quantity = 2 };

            A.CallTo(() => fakeService.CreateOrderItemAsync(input))
                .Throws(new InvalidOperationException("Invalid item"));

            var controller = new OrderItemsController(fakeService);
            var result = await controller.Create(input);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid item", badRequest.Value);
        }

        [Fact]
        public async Task Update_Returns_NotFound_When_KeyNotFound()
        {
            var fakeService = A.Fake<IOrderItemService>();
            var dto = new OrderItemDto { Id = 1, OrderId = 10 };

            A.CallTo(() => fakeService.UpdateOrderItemAsync(1, dto))
                .Throws(new KeyNotFoundException());

            var controller = new OrderItemsController(fakeService);
            var result = await controller.Update(1, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_NoContent_When_Successful()
        {
            var fakeService = A.Fake<IOrderItemService>();

            A.CallTo(() => fakeService.DeleteOrderItemAsync(1)).Returns(Task.CompletedTask);

            var controller = new OrderItemsController(fakeService);
            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_NotFound_When_KeyNotFound()
        {
            var fakeService = A.Fake<IOrderItemService>();

            A.CallTo(() => fakeService.DeleteOrderItemAsync(1)).Throws(new KeyNotFoundException());

            var controller = new OrderItemsController(fakeService);
            var result = await controller.Delete(1);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
