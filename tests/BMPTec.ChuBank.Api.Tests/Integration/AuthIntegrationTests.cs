using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace BMPTec.ChuBank.Api.Tests.Integration
{
    public class AuthIntegrationTests : IntegrationTestBase
    {
        public AuthIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var loginRequest = new
            {
                Username = "admin",
                Password = "123456"
            };

            // Act
            var response = await _client.PostAsync("/api/v1.0/auth/login", CreateJsonContent(loginRequest));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(content);
            
            Assert.True(tokenResponse.TryGetProperty("token", out var tokenProperty));
            Assert.False(string.IsNullOrEmpty(tokenProperty.GetString()));
        }

        [Theory]
        [InlineData("admin", "wrongpassword")]
        [InlineData("wronguser", "123456")]
        [InlineData("", "123456")]
        [InlineData("admin", "")]
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

        [Fact]
        public async Task Login_WithMalformedRequest_ShouldReturnBadRequest()
        {
            // Arrange
            var malformedJson = "{ invalid json }";
            var content = new StringContent(malformedJson, System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1.0/auth/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithEmptyBody_ShouldReturnBadRequest()
        {
            // Arrange
            var emptyContent = new StringContent("", System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1.0/auth/login", emptyContent);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
