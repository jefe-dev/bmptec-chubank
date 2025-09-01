using BMPTec.ChuBank.Api.Controllers.v1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace BMPTec.ChuBank.Api.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            SetupConfiguration();
            _controller = new AuthController(_configurationMock.Object);
        }

        private void SetupConfiguration()
        {
            _configurationMock.Setup(x => x["Jwt:Key"]).Returns("super-secret-key-with-at-least-256-bits-for-security-purposes");
            _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("ChuBank.Api");
            _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("ChuBank.Users");
        }

        [Fact]
        public void Login_WithValidCredentials_ShouldReturnOk()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "123456"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "wrong",
                Password = "wrong"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public void Login_WithValidCredentials_ShouldReturnValidToken()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "admin",
                Password = "123456"
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.NotNull(okResult.Value);
            
            // Verify it contains a token property
            var tokenProperty = okResult.Value.GetType().GetProperty("token");
            Assert.NotNull(tokenProperty);
            
            var tokenValue = tokenProperty.GetValue(okResult.Value)?.ToString();
            Assert.NotNull(tokenValue);
            Assert.NotEmpty(tokenValue);
        }

        [Theory]
        [InlineData("admin", "wrongpassword")]
        [InlineData("wronguser", "123456")]
        [InlineData("", "123456")]
        [InlineData("admin", "")]
        public void Login_WithInvalidCredentials_ShouldReturnUnauthorizedMessage(string username, string password)
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = username,
                Password = password
            };

            // Act
            var result = _controller.Login(loginRequest);

            // Assert
            var unauthorizedResult = result as UnauthorizedObjectResult;
            Assert.NotNull(unauthorizedResult);
            Assert.Equal("Usuário ou senha inválidos", unauthorizedResult.Value);
        }
    }
}
