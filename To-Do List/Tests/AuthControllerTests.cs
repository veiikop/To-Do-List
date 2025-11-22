using Moq;
using Xunit;
using To_Do_List.Controllers;
using To_Do_List.Services;
using To_Do_List.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace To_Do_List.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ILogger<AuthController>> _mockLogger;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockLogger = new Mock<ILogger<AuthController>>();
            _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);
        }

        [Fact]
        public void Register_ValidData_ReturnsCreatedResultWithToken()
        {
            // Arrange
            var request = new RegisterRequestDTO
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123"
            };

            var response = new AuthResponseDTO
            {
                Success = true,
                Token = "fake-jwt-token",
                ValidTo = DateTime.UtcNow.AddHours(1),
                User = new UserDTO { Id = 1, Username = "testuser", Email = "test@example.com", Role = "User" }
            };

            _mockAuthService.Setup(s => s.Register(request)).Returns(response);

            // Act
            var result = _controller.Register(request);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<AuthResponseDTO>(createdResult.Value);
            Assert.True(returnValue.Success);
            Assert.NotEmpty(returnValue.Token);
        }

        [Fact]
        public void Register_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var request = new RegisterRequestDTO { Username = "", Email = "invalid", Password = "123" };
            _controller.ModelState.AddModelError("Username", "Required");

            // Act
            var result = _controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuthResponseDTO>(badRequestResult.Value);
            Assert.False(returnValue.Success);
            Assert.Equal("некорректные данные", returnValue.ErrorMessage);
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var request = new LoginRequestDTO { EmailOrUsername = "testuser", Password = "Password123" };
            var response = new AuthResponseDTO
            {
                Success = true,
                Token = "fake-jwt-token",
                User = new UserDTO { Id = 1, Username = "testuser" }
            };

            _mockAuthService.Setup(s => s.Login(request)).Returns(response);

            // Act
            var result = _controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuthResponseDTO>(okResult.Value);
            Assert.True(returnValue.Success);
            Assert.NotEmpty(returnValue.Token);
        }

        [Fact]
        public void Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var request = new LoginRequestDTO { EmailOrUsername = "wrong", Password = "wrong" };
            var response = new AuthResponseDTO { Success = false, ErrorMessage = "Неверный пароль" };

            _mockAuthService.Setup(s => s.Login(request)).Returns(response);

            // Act
            var result = _controller.Login(request);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var returnValue = Assert.IsType<AuthResponseDTO>(unauthorizedResult.Value);
            Assert.False(returnValue.Success);
        }
    }
}