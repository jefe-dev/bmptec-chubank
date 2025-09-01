using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using Xunit;

namespace BMPTec.ChuBank.Api.Tests.Integration
{
    public class AuthIntegrationTests : IntegrationTestBase
    {
        public AuthIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnOk()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "admin",
                Password = "admin123"
            };

            // Act
            var response = await _client.PostAsync("/api/v1.0/auth/login", CreateJsonContent(loginRequest));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            Assert.True(result.TryGetProperty("token", out _));
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnValidToken()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "admin",
                Password = "admin123"
            };

            // Act
            var response = await _client.PostAsync("/api/v1.0/auth/login", CreateJsonContent(loginRequest));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(content);
            var token = result.GetProperty("token").GetString();
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Theory]
        [InlineData("admin", "wrongpass123")] 
        [InlineData("wronguser", "admin123")] 
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized(string username, string password)
        {
            // Arrange
            var loginRequest = new
            {
                Username = username,
                Password = password
            };

            // Act
            var response = await _client.PostAsync("/api/v1.0/auth/login", CreateJsonContent(loginRequest));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("", "admin123")] // Empty username
        [InlineData("admin", "")] // Empty password
        [InlineData("ad", "admin123")] // Username too short
        [InlineData("admin", "123")] // Password too short
        [InlineData("admin", "123456")] // Password too simple (only numbers)
        public async Task Login_WithInvalidFormat_ShouldReturnBadRequest(string username, string password)
        {
            // Arrange
            var loginRequest = new
            {
                Username = username,
                Password = password
            };

            // Act
            var response = await _client.PostAsync("/api/v1.0/auth/login", CreateJsonContent(loginRequest));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
