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
    public class NotificationsControllerTests
    {
        [Fact]
        public async Task GetAll_Returns_Ok()
        {
            var fakeService = A.Fake<INotificationService>();
            var expected = new List<NotificationDto> { new NotificationDto { Id = 1 } };

            A.CallTo(() => fakeService.GetAllNotificationsAsync()).Returns(expected);

            var controller = new NotificationsController(fakeService);
            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<NotificationDto>>(ok.Value);
            Assert.Single(data);
        }

        [Fact]
        public async Task GetById_Returns_Ok_When_Found()
        {
            var fakeService = A.Fake<INotificationService>();
            var dto = new NotificationDto { Id = 1 };

            A.CallTo(() => fakeService.GetNotificationByIdAsync(1)).Returns(dto);

            var controller = new NotificationsController(fakeService);
            var result = await controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(dto, ok.Value);
        }


        [Fact]
        public async Task GetByUser_Returns_Ok()
        {
            var fakeService = A.Fake<INotificationService>();
            var expected = new List<NotificationDto> { new NotificationDto { Id = 1, UserId = 5 } };

            A.CallTo(() => fakeService.GetNotificationsByUserIdAsync(5)).Returns(expected);

            var controller = new NotificationsController(fakeService);
            var result = await controller.GetByUser(5);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<NotificationDto>>(ok.Value);
            Assert.Single(data);
        }

        [Fact]
        public async Task Create_Returns_CreatedAt()
        {
            var fakeService = A.Fake<INotificationService>();
            var input = new NotificationDto { Message = "Test" };
            var created = new NotificationDto { Id = 1, Message = "Test" };

            A.CallTo(() => fakeService.CreateNotificationAsync(input)).Returns(created);

            var controller = new NotificationsController(fakeService);
            var result = await controller.Create(input);

            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created.Id, ((NotificationDto)createdAt.Value).Id);
        }

        [Fact]
        public async Task Update_Returns_NoContent_When_Successful()
        {
            var fakeService = A.Fake<INotificationService>();
            var dto = new NotificationDto { Id = 1 };

            A.CallTo(() => fakeService.UpdateNotificationAsync(1, dto)).Returns(Task.CompletedTask);

            var controller = new NotificationsController(fakeService);
            var result = await controller.Update(1, dto);

            Assert.IsType<NoContentResult>(result);
        }


        [Fact]
        public async Task Update_Returns_NotFound_When_NotExists()
        {
            var fakeService = A.Fake<INotificationService>();
            var dto = new NotificationDto { Id = 1 };

            A.CallTo(() => fakeService.UpdateNotificationAsync(1, dto)).Throws(new KeyNotFoundException());

            var controller = new NotificationsController(fakeService);
            var result = await controller.Update(1, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_NoContent()
        {
            var fakeService = A.Fake<INotificationService>();
            A.CallTo(() => fakeService.DeleteNotificationAsync(1)).Returns(Task.CompletedTask);

            var controller = new NotificationsController(fakeService);
            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_NotFound_When_NotExists()
        {
            var fakeService = A.Fake<INotificationService>();
            A.CallTo(() => fakeService.DeleteNotificationAsync(1)).Throws(new KeyNotFoundException());

            var controller = new NotificationsController(fakeService);
            var result = await controller.Delete(1);

            Assert.IsType<NotFoundResult>(result);
        }

    }
}
