using BMPTec.ChuBank.Api.Controllers.v1;
using BMPTec.ChuBank.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text.Json;
using Xunit;

namespace BMPTec.ChuBank.Api.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly AuthController _controller;
        private readonly Mock<IConfiguration> _mockConfiguration;

        public AuthControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("super-secret-key-that-is-long-enough-for-hs256-algorithm-123456789");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("test-issuer");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("test-audience");

            _controller = new AuthController(_mockConfiguration.Object);
        }

        [Fact]
        public void Login_WithValidCredentials_ShouldReturnOk()
        {
            // Arrange
            var loginRequest = new LoginRequestDto("admin", "admin123");

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public void Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequestDto("wrong", "wrong");

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password", unauthorizedResult.Value);
        }

        [Fact]
        public void Login_WithValidCredentials_ShouldReturnValidToken()
        {
            // Arrange
            var loginRequest = new LoginRequestDto("admin", "admin123");

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var jsonString = JsonSerializer.Serialize(okResult.Value);
            var response = JsonSerializer.Deserialize<JsonElement>(jsonString);
            Assert.True(response.TryGetProperty("token", out var tokenProperty));
            var token = tokenProperty.GetString();
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Theory]
        [InlineData("admin", "wrongpass")]
        [InlineData("wronguser", "admin123")]
        [InlineData("", "admin123")]
        [InlineData("admin", "")]
        public void Login_WithInvalidCredentials_ShouldReturnUnauthorizedMessage(string username, string password)
        {
            // Arrange
            var loginRequest = new LoginRequestDto(username, password);

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username or password", unauthorizedResult.Value);
        }
    }
}
