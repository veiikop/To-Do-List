using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using To_Do_List.Controllers;
using To_Do_List.Services;
using To_Do_List.Models.DTO;
using To_Do_List.Models;

namespace To_Do_List.Tests
{
    public class TodoItemsControllerTests
    {
        private readonly Mock<ITodoItemService> _mockService;
        private readonly TodoItemsController _controller;

        public TodoItemsControllerTests()
        {
            _mockService = new Mock<ITodoItemService>();

            // Настраиваем авторизованного пользователя (обычный User, Id = 1)
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "User")
            }, "mock"));

            _controller = new TodoItemsController(_mockService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext { User = user }
                }
            };
        }

        // GET: api/TodoItems/{id}
        [Fact]
        public void GetById_ExistingId_ReturnsOkObjectResult()
        {
            // Arrange
            var expectedItem = new TodoItemDTO
            {
                Id = 1,
                Title = "Купить молоко",
                TodoListId = 1
            };

            _mockService.Setup(s => s.GetById(1)).Returns(expectedItem);

            // Act
            var result = _controller.GetById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedItem = Assert.IsType<TodoItemDTO>(okResult.Value);
            Assert.Equal(expectedItem.Id, returnedItem.Id);
            Assert.Equal(expectedItem.Title, returnedItem.Title);
        }

        [Fact]
        public void GetById_NonExistingId_ReturnsNotFoundObjectResult()
        {
            // Arrange
            _mockService.Setup(s => s.GetById(999)).Returns((TodoItemDTO?)null);

            // Act
            var result = _controller.GetById(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = notFoundResult.Value;

            // Проверяем структуру ответа { Success = false, ErrorMessage = "..." }
            var successProp = response?.GetType().GetProperty("Success");
            var errorProp = response?.GetType().GetProperty("ErrorMessage");

            Assert.NotNull(successProp);
            Assert.NotNull(errorProp);
            Assert.False((bool)successProp!.GetValue(response)!);
            Assert.Contains("не найдена", errorProp!.GetValue(response) as string);
        }

        // POST: api/TodoItems
        [Fact]
        public void Create_ValidItem_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var createDto = new CreateTodoItemDTO
            {
                Title = "Новая задача",
                TodoListId = 1,
                Priority = Priority.High
            };

            var createdItem = new TodoItemDTO
            {
                Id = 5,
                Title = "Новая задача",
                TodoListId = 1,
                Priority = Priority.High
            };

            _mockService.Setup(s => s.Create(createDto)).Returns(createdItem);

            // Act
            var result = _controller.Create(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(TodoItemsController.GetById), createdAtActionResult.ActionName);
            Assert.Equal(5, createdAtActionResult.RouteValues?["id"]);

            var returnedItem = Assert.IsType<TodoItemDTO>(createdAtActionResult.Value);
            Assert.Equal("Новая задача", returnedItem.Title);
        }

        // PUT: api/TodoItems/{id}
        [Fact]
        public void Update_ExistingIdAndValidData_ReturnsOkObjectResult()
        {
            // Arrange
            var updateDto = new UpdateTodoItemDTO
            {
                Title = "Обновлённое название",
                IsCompleted = true
            };

            var updatedItem = new TodoItemDTO
            {
                Id = 3,
                Title = "Обновлённое название",
                IsCompleted = true
            };

            _mockService.Setup(s => s.Update(3, updateDto)).Returns(updatedItem);

            // Act
            var result = _controller.Update(3, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedItem = Assert.IsType<TodoItemDTO>(okResult.Value);
            Assert.True(returnedItem.IsCompleted);
            Assert.Equal("Обновлённое название", returnedItem.Title);
        }

        [Fact]
        public void Update_NonExistingId_ReturnsNotFoundObjectResult()
        {
            // Arrange
            var updateDto = new UpdateTodoItemDTO { Title = "Не важно" };
            _mockService.Setup(s => s.Update(999, updateDto)).Returns((TodoItemDTO?)null);

            // Act
            var result = _controller.Update(999, updateDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = notFoundResult.Value;
            Assert.NotNull(response?.GetType().GetProperty("Success"));
            Assert.False((bool)response!.GetType().GetProperty("Success")!.GetValue(response)!);
        }

        // DELETE: api/TodoItems/{id}
        [Fact]
        public void Delete_ExistingId_ReturnsNoContentResult()
        {
            // Arrange
            _mockService.Setup(s => s.Delete(7)).Returns(true);

            // Act
            var result = _controller.Delete(7);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void Delete_NonExistingId_ReturnsNotFoundObjectResult()
        {
            // Arrange
            _mockService.Setup(s => s.Delete(999)).Returns(false);

            // Act
            var result = _controller.Delete(999);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = notFoundResult.Value;
            Assert.False((bool)response!.GetType().GetProperty("Success")!.GetValue(response)!);
        }

        // GET: api/TodoItems/by-list/{todoListId}
        [Fact]
        public void GetItemsByListId_ValidListId_ReturnsOkWithListOfItems()
        {
            // Arrange
            var items = new List<TodoItemDTO>
            {
                new TodoItemDTO { Id = 10, Title = "Задача 1", TodoListId = 2 },
                new TodoItemDTO { Id = 11, Title = "Задача 2", TodoListId = 2 }
            };

            _mockService.Setup(s => s.GetItemsByListId(2)).Returns(items);

            // Act
            var result = _controller.GetItemsByListId(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedItems = Assert.IsType<List<TodoItemDTO>>(okResult.Value);
            Assert.Equal(2, returnedItems.Count);
        }
    }
}