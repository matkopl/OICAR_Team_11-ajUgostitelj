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
    public class OrdersControllerTests
    {
        [Fact]
        public async Task GetAll_Returns_Ok()
        {
            // Arrange
            //fejk service
            var fakeOrderService = A.Fake<IOrderService>();

            //simulacija vracenih ordera sa fejk servisa
            var expectedOrders = new List<OrderDto>
            {
                new OrderDto
                {
                    Id = 1,
                    OrderDate = new DateTime(2023, 1, 1),
                    Status = "Pending",
                    TotalAmount = 100.00m,
                    TableId = 1
                },
                new OrderDto
                {
                    Id = 2,
                    OrderDate = new DateTime(2023, 1, 2),
                    Status = "Completed",
                    TotalAmount = 200.00m,
                    TableId = 2
                }
            };

            A.CallTo(() => fakeOrderService.GetAllOrdersAsync())
                .Returns(Task.FromResult<IEnumerable<OrderDto>>(expectedOrders));

            var controller = new OrderController(fakeOrderService);

            // zovemo metodu na kontroleru
            var result = await controller.GetAll();
            
            //vraca ok
            var ok = Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetById_Returns_Ok()
        {
            // Arrange
            //simuliramo podatke 
            int testOrderId = 1;
            var expectedOrder = new OrderDto
            {
                Id = testOrderId,
                OrderDate = new DateTime(2023, 1, 1),
                Status = "Pending",
                TotalAmount = 100.00m,
                TableId = 2
            };

            var fakeOrderService = A.Fake<IOrderService>();
            A.CallTo(() => fakeOrderService.GetOrderByIdAsync(testOrderId))
                .Returns(Task.FromResult<OrderDto?>(expectedOrder));

            var controller = new OrderController(fakeOrderService);

            // zovemo metodu na kontroleru
            var result = await controller.GetById(testOrderId);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedOrder = Assert.IsType<OrderDto>(okResult.Value);
            Assert.Equal(expectedOrder.Id, returnedOrder.Id);
            Assert.Equal(expectedOrder.Status, returnedOrder.Status);
        }
        
        [Fact]
        public async Task Create_Test_Successful()
        {
            // simulirani servis
            var fakeOrderService = A.Fake<IOrderService>();

            // simuliramo order
            var inputOrder = new OrderDto
            {
                Id = 0,
                OrderDate = new DateTime(2023, 1, 1),
                Status = "Pending",
                TotalAmount = 150.00m,
                TableId = 3
            };

            // order dobiva id 1 u bazi npr
            var createdOrder = new OrderDto
            {
                Id = 1,  // Assigned by the service/database
                OrderDate = inputOrder.OrderDate,
                Status = inputOrder.Status,
                TotalAmount = inputOrder.TotalAmount,
                TableId = inputOrder.TableId
            };

            A.CallTo(() => fakeOrderService.CreateOrderAsync(inputOrder))
                .Returns(Task.FromResult(createdOrder));


            var controller = new OrderController(fakeOrderService);

            // zovemo metodu na kontroleru
            var result = await controller.Create(inputOrder);


            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result.Result);


            // usporedba
            var returnedOrder = Assert.IsType<OrderDto>(createdAtResult.Value);
            Assert.Equal(createdOrder.Id, returnedOrder.Id);
            Assert.Equal(createdOrder.Status, returnedOrder.Status);
            Assert.Equal(createdOrder.TotalAmount, returnedOrder.TotalAmount);
            Assert.Equal(createdOrder.TableId, returnedOrder.TableId);
        }

        [Fact]
        public async Task Delete_Successful()
        {
            // simulacija id
            int orderId = 1;
            var fakeOrderService = A.Fake<IOrderService>();
           

            // simuliran uspjesan delete
            A.CallTo(() => fakeOrderService.DeleteOrderAsync(orderId))
                .Returns(Task.CompletedTask);
            var controller = new OrderController(fakeOrderService);

            // Act
            var result = await controller.Delete(orderId);

            // rezultat
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_Successful()
        {
            // Arrange
            int id = 1;
            var orderDto = new OrderDto { Id = 1, OrderDate = DateTime.Now, Status = "Pending", TotalAmount = 100m, TableId = 3 };
            var fakeService = A.Fake<IOrderService>();

            // simulacija uspjesnog update na fake servisu
            A.CallTo(() => fakeService.UpdateOrderAsync(id, orderDto))
                .Returns(Task.CompletedTask);

            // metoda na kontroleru
            var controller = new OrderController(fakeService);
            var result = await controller.Update(id, orderDto);

            //rezuiltat
            Assert.IsType<NoContentResult>(result);
        }
    }
}
