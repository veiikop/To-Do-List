using Moq;
using Xunit;
using To_Do_List.Controllers;
using To_Do_List.Services;
using To_Do_List.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace To_Do_List.Tests
{
    public class TodoListsControllerTests
    {
        private readonly Mock<ITodoListService> _mockService;
        private readonly TodoListsController _controller;

        public TodoListsControllerTests()
        {
            _mockService = new Mock<ITodoListService>();
            _controller = new TodoListsController(_mockService.Object);

            // Настраиваем User в HttpContext (для авторизации)
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public void GetAll_ReturnsListOfTodoLists()
        {
            // Arrange
            var lists = new List<TodoListDTO>
            {
                new TodoListDTO { Id = 1, Title = "My List" }
            };
            _mockService.Setup(s => s.GetAll()).Returns(lists);

            // Act
            var result = _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<TodoListDTO>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public void Create_ValidList_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateTodoListDTO { Title = "New List" };
            var createdList = new TodoListDTO { Id = 1, Title = "New List" };

            _mockService.Setup(s => s.Create(dto)).Returns(createdList);

            // Act
            var result = _controller.Create(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetById", createdAtActionResult.ActionName);
            var returnValue = Assert.IsType<TodoListDTO>(createdAtActionResult.Value);
            Assert.Equal(1, returnValue.Id);
        }
    }
}