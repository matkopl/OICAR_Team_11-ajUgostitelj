using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Controllers;
using WebAPI.DTOs;
using WebAPI.Models;
using WebAPI.Repository;
using WebAPI.Services;
using Xunit;

namespace UnitTestProject
{
    public class TablesControllerTests
    {
        [Fact]
        public async Task GetAll_Method_Returns_TablesAsync()
        {
            //Arrange
            
            var fakeService = A.Fake<ITableService>();

            var fakeTables = new List<TableDto>
            {
                new TableDto { Id = 1, Name = "Test Table 1", IsOccupied = false},
                new TableDto { Id = 2, Name = "Test Table 2", IsOccupied = true}
            };

            A.CallTo(() => fakeService.GetAllTablesAsync())
               .Returns(Task.FromResult<IEnumerable<TableDto>>(fakeTables));

            var controller = new TableController(fakeService);

            //Act

            var result = await controller.GetAllTables();

            //Assert

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var okTables = Assert.IsAssignableFrom<IEnumerable<TableDto>>(okResult.Value);
            Assert.Equal(fakeTables.Count, okTables.Count());
        }

        
        
        [Fact]
        public async Task DeleteTable_Returns_NoContent_When_DeletionIsSuccessful()
        {
            // Arrange
            int testId = 1;
            var fakeService = A.Fake<ITableService>();
            
            // simulacija brisanja uspješna
            A.CallTo(() => fakeService.DeleteTableAsync(testId))
                .Returns(Task.CompletedTask);

            var controller = new TableController(fakeService);

            // Act
            var result = await controller.DeleteTable(testId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteTable_Returns_NotFound_When_TableNotFound()
        {
            // Arrange
            int testId = 99;
            var fakeService = A.Fake<ITableService>();
            
            // stol nije nađen
            A.CallTo(() => fakeService.DeleteTableAsync(testId))
                .ThrowsAsync(new KeyNotFoundException("Table not found"));

            var controller = new TableController(fakeService);

            // Act
            var result = await controller.DeleteTable(testId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteTable_Returns_StatusCode500_When_UnexpectedErrorOccurs()
        {
            // Arrange
            int testId = 1;
            var fakeService = A.Fake<ITableService>();
            
            //simulacija exceptiona
            A.CallTo(() => fakeService.DeleteTableAsync(testId))
                .ThrowsAsync(new Exception("Unexpected error"));

            var controller = new TableController(fakeService);

            // Act
            var result = await controller.DeleteTable(testId);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateTable_Returns_CreatedAtActionResult_When_Successful()
        {
            // Arrange
            var fakeService = A.Fake<ITableService>();

            //upis kroz API
            var inputDto = new TableDto { Id = 0, Name = "New Table", IsOccupied = false };

            //fejk podatak u bazi
            var createdDto = new TableDto { Id = 1, Name = "New Table", IsOccupied = false };

            
            A.CallTo(() => fakeService.CreateTableAsync(inputDto))
                .Returns(Task.FromResult(createdDto));

            var controller = new TableController(fakeService);

            // poziv metode na kontroleru
            var result = await controller.CreateTable(inputDto);

            // rezultati
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedDto = Assert.IsType<TableDto>(createdResult.Value);

            // usporedba
            Assert.Equal(createdDto.Id, returnedDto.Id);
            Assert.Equal(createdDto.Name, returnedDto.Name);
            Assert.Equal(createdDto.IsOccupied, returnedDto.IsOccupied);
        }

        [Fact]
        public async Task UpdateTable_Returns_Successful()
        {
            // Arrange
            int urlId = 1;
            var tableDto = new TableDto { Id = 1, Name = "Table", IsOccupied = false };
            var fakeService = A.Fake<ITableService>();

            // simuliramo uspjesan update
            A.CallTo(() => fakeService.UpdateTableAsync(urlId, tableDto))
                .Returns(Task.CompletedTask);

            var controller = new TableController(fakeService);

            // Act
            var result = await controller.UpdateTable(urlId, tableDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
        [Fact]
        public async Task UpdateTable_Returns_Not_Successful()
        {
            // Arrange
            int urlId = 1;
            var tableDto = new TableDto { Id = 1, Name = "Table", IsOccupied = false };
            var fakeService = A.Fake<ITableService>();

            // simuliramo execption 
            A.CallTo(() => fakeService.UpdateTableAsync(urlId, tableDto))
                .Throws(new Exception("Pad"));

            var controller = new TableController(fakeService);

            // Act
            var result = await controller.UpdateTable(urlId, tableDto);

            // Assert
            var rezultat = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500,rezultat.StatusCode);
        }



    }
}