using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI;
using WebAPI.Controllers;
using WebAPI.DTOs;
using WebAPI.Hubs;
using WebAPI.Services;

namespace UnitTestProject
{
    public class OrdersControllerTests
    {
        [Fact]
        public async Task GetAll_Returns_Ok_With_Data()
        {
            var fakeService = A.Fake<IOrderService>();
            var orders = new List<OrderDto>
            {
                new OrderDto { Id = 1, Status = OrderStatus.Pending },
                new OrderDto { Id = 2, Status = OrderStatus.Completed }
            };
            A.CallTo(() => fakeService.GetAllOrdersAsync()).Returns(orders);

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<OrderDto>>(okResult.Value);
            Assert.Equal(2, data.Count());
        }

        [Fact]
        public async Task GetAll_Returns_Ok_With_EmptyList()
        {
            var fakeService = A.Fake<IOrderService>();
            A.CallTo(() => fakeService.GetAllOrdersAsync()).Returns(new List<OrderDto>());

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<OrderDto>>(okResult.Value);
            Assert.Empty(data);
        }

        [Fact]
        public async Task GetAll_Returns_BadRequest_On_Exception()
        {
            var fakeService = A.Fake<IOrderService>();
            A.CallTo(() => fakeService.GetAllOrdersAsync()).Throws(new Exception("Database error"));

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.GetAll();

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Database error", badRequestResult.Value);
        }

        [Fact]
        public async Task GetById_Returns_Ok_When_Order_Exists()
        {
            var fakeService = A.Fake<IOrderService>();
            var order = new OrderDto { Id = 1, Status = OrderStatus.Pending };

            A.CallTo(() => fakeService.GetOrderByIdAsync(1)).Returns(order);

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(order, okResult.Value);
        }

        [Fact]
        public async Task GetById_Returns_NotFound_When_Order_Does_Not_Exist()
        {
            var fakeService = A.Fake<IOrderService>();
            A.CallTo(() => fakeService.GetOrderByIdAsync(99)).Returns((OrderDto?)null);

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.GetById(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetById_Returns_BadRequest_On_Exception()
        {
            var fakeService = A.Fake<IOrderService>();
            A.CallTo(() => fakeService.GetOrderByIdAsync(1)).Throws(new Exception("Database error"));

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.GetById(1);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Database error", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_Returns_NoContent_When_Successful()
        {
            var fakeService = A.Fake<IOrderService>();
            var orderDto = new OrderDto { Id = 1, Status = OrderStatus.Pending };

            A.CallTo(() => fakeService.UpdateOrderAsync(1, orderDto)).Returns(Task.CompletedTask);

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.Update(1, orderDto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_Returns_NotFound_When_Order_Does_Not_Exist()
        {
            var fakeService = A.Fake<IOrderService>();
            var orderDto = new OrderDto { Id = 99, Status = OrderStatus.Pending };

            A.CallTo(() => fakeService.UpdateOrderAsync(99, orderDto)).Throws(new KeyNotFoundException());

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.Update(99, orderDto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_Returns_BadRequest_When_ID_Mismatch()
        {
            var fakeService = A.Fake<IOrderService>();
            var orderDto = new OrderDto { Id = 2, Status = OrderStatus.Pending };

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.Update(1, orderDto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID in URL does not match ID in body", badRequestResult.Value);
        }

        [Fact]
        public async Task Update_Returns_InternalServerError_On_Exception()
        {
            var fakeService = A.Fake<IOrderService>();
            var orderDto = new OrderDto { Id = 1, Status = OrderStatus.Pending };

            A.CallTo(() => fakeService.UpdateOrderAsync(1, orderDto)).Throws(new Exception("Database error"));

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.Update(1, orderDto);

            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            Assert.Equal("Error occurred", serverErrorResult.Value);
        }

        [Fact]
        public async Task Delete_Returns_NoContent_When_Successful()
        {
            var fakeService = A.Fake<IOrderService>();
            A.CallTo(() => fakeService.DeleteOrderAsync(1)).Returns(Task.CompletedTask);

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_NotFound_When_Order_Does_Not_Exist()
        {
            var fakeService = A.Fake<IOrderService>();
            A.CallTo(() => fakeService.DeleteOrderAsync(99)).Throws(new KeyNotFoundException());

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.Delete(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_InternalServerError_On_Exception()
        {
            var fakeService = A.Fake<IOrderService>();
            A.CallTo(() => fakeService.DeleteOrderAsync(1)).Throws(new Exception("Database error"));

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.Delete(1);

            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            Assert.Equal("Error with request!", serverErrorResult.Value);
        }

        [Fact]
        public async Task GetStatus_Returns_Ok_When_Order_Exists()
        {
            var fakeService = A.Fake<IOrderService>();
            var expectedStatus = "Completed";

            A.CallTo(() => fakeService.GetOrderStatusAsync(1)).Returns(expectedStatus);

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.GetStatus(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var statusObject = okResult.Value;
            Assert.NotNull(statusObject);
            Assert.Equal(expectedStatus, statusObject.GetType().GetProperty("Status")?.GetValue(statusObject, null)); // ✅ Dinamičko dohvaćanje
        }
        [Fact]
        public async Task GetStatus_Returns_NotFound_When_Order_Does_Not_Exist()
        {
            var fakeService = A.Fake<IOrderService>();
            A.CallTo(() => fakeService.GetOrderStatusAsync(99)).Throws(new KeyNotFoundException());

            var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

            var result = await controller.GetStatus(99);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
public async Task UpdateStatus_Returns_NoContent_When_Successful()
{
    var fakeService = A.Fake<IOrderService>();
    var statusDto = new OrderStatusDto { OrderId = 1, Status = OrderStatus.Completed };

    A.CallTo(() => fakeService.UpdateOrderStatusAsync(statusDto)).Returns(Task.CompletedTask);

    var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

    var result = await controller.UpdateStatus(1, statusDto);

    Assert.IsType<NoContentResult>(result);
}

[Fact]
public async Task UpdateStatus_Returns_BadRequest_When_ID_Mismatch()
{
    var fakeService = A.Fake<IOrderService>();
    var statusDto = new OrderStatusDto { OrderId = 2, Status = OrderStatus.Pending };

    var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

    var result = await controller.UpdateStatus(1, statusDto);

    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Order ID mismatch", badRequestResult.Value);
}

[Fact]
public async Task UpdateStatus_Returns_BadRequest_On_Exception()
{
    var fakeService = A.Fake<IOrderService>();
    var statusDto = new OrderStatusDto { OrderId = 1, Status = OrderStatus.Pending };

    A.CallTo(() => fakeService.UpdateOrderStatusAsync(statusDto)).Throws(new Exception("Database error"));

    var controller = new OrderController(fakeService, A.Fake<IHubContext<OrderHub>>());

    var result = await controller.UpdateStatus(1, statusDto);

    var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    Assert.Equal("Database error", badRequestResult.Value);
}
    }
}
