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
    public class RolesControllerTests
    {
        [Fact]
        public async Task GetAll_Returns_Ok()
        {
            var fakeService = A.Fake<IRoleService>();
            var roles = new List<RoleDto> { new RoleDto { Id = 1, Name = "Admin" } };

            A.CallTo(() => fakeService.GetAllRolesAsync()).Returns(roles);

            var controller = new RolesController(fakeService);
            var result = await controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsAssignableFrom<IEnumerable<RoleDto>>(ok.Value);
            Assert.Single(data);
        }

        [Fact]
        public async Task GetById_Returns_Ok_When_Found()
        {
            var fakeService = A.Fake<IRoleService>();
            var role = new RoleDto { Id = 1, Name = "User" };

            A.CallTo(() => fakeService.GetRoleByIdAsync(1)).Returns(role);

            var controller = new RolesController(fakeService);
            var result = await controller.GetById(1);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(role, ok.Value);
        }

        [Fact]
        public async Task GetById_Returns_NotFound_When_Null()
        {
            var fakeService = A.Fake<IRoleService>();

            A.CallTo(() => fakeService.GetRoleByIdAsync(1)).Returns((RoleDto?)null);

            var controller = new RolesController(fakeService);
            var result = await controller.GetById(1);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task Create_Returns_CreatedAt_When_Successful()
        {
            var fakeService = A.Fake<IRoleService>();
            var input = new RoleDto { Name = "Manager" };
            var created = new RoleDto { Id = 1, Name = "Manager" };

            A.CallTo(() => fakeService.CreateRoleAsync(input)).Returns(created);

            var controller = new RolesController(fakeService);
            var result = await controller.Create(input);

            var createdAt = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(created.Id, ((RoleDto)createdAt.Value).Id);
        }

        [Fact]
        public async Task Create_Returns_BadRequest_On_InvalidOperationException()
        {
            var fakeService = A.Fake<IRoleService>();
            var input = new RoleDto { Name = "Duplicate" };

            A.CallTo(() => fakeService.CreateRoleAsync(input))
                .Throws(new InvalidOperationException("Role already exists"));

            var controller = new RolesController(fakeService);
            var result = await controller.Create(input);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Role already exists", badRequest.Value);
        }

        [Fact]
        public async Task Update_Returns_NoContent_When_Successful()
        {
            var fakeService = A.Fake<IRoleService>();
            var dto = new RoleDto { Id = 1, Name = "Editor" };

            A.CallTo(() => fakeService.UpdateRoleAsync(1, dto)).Returns(Task.CompletedTask);

            var controller = new RolesController(fakeService);
            var result = await controller.Update(1, dto);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Update_Returns_NotFound_On_KeyNotFoundException()
        {
            var fakeService = A.Fake<IRoleService>();
            var dto = new RoleDto { Id = 1, Name = "Missing" };

            A.CallTo(() => fakeService.UpdateRoleAsync(1, dto))
                .Throws(new KeyNotFoundException());

            var controller = new RolesController(fakeService);
            var result = await controller.Update(1, dto);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Update_Returns_BadRequest_On_InvalidOperationException()
        {
            var fakeService = A.Fake<IRoleService>();
            var dto = new RoleDto { Id = 1, Name = "Invalid" };

            A.CallTo(() => fakeService.UpdateRoleAsync(1, dto))
                .Throws(new InvalidOperationException("Invalid role update"));

            var controller = new RolesController(fakeService);
            var result = await controller.Update(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid role update", badRequest.Value);
        }

        [Fact]
        public async Task Delete_Returns_NoContent_When_Successful()
        {
            var fakeService = A.Fake<IRoleService>();

            A.CallTo(() => fakeService.DeleteRoleAsync(1)).Returns(Task.CompletedTask);

            var controller = new RolesController(fakeService);
            var result = await controller.Delete(1);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_NotFound_On_KeyNotFoundException()
        {
            var fakeService = A.Fake<IRoleService>();

            A.CallTo(() => fakeService.DeleteRoleAsync(1)).Throws(new KeyNotFoundException());

            var controller = new RolesController(fakeService);
            var result = await controller.Delete(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Returns_BadRequest_On_InvalidOperationException()
        {
            var fakeService = A.Fake<IRoleService>();

            A.CallTo(() => fakeService.DeleteRoleAsync(1))
                .Throws(new InvalidOperationException("Role is assigned to users"));

            var controller = new RolesController(fakeService);
            var result = await controller.Delete(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Role is assigned to users", badRequest.Value);
        }
    }
}
