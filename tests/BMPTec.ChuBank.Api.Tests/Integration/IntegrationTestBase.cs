using BMPTec.ChuBank.Api.Data;
using BMPTec.ChuBank.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BMPTec.ChuBank.Api.Tests.Integration
{
    public class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly WebApplicationFactory<Program> _factory;
        protected readonly HttpClient _client;

        public IntegrationTestBase(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the real database
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Add in-memory database
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                    });

                    // Mock Holiday Service for tests
                    var mockHolidayService = new Mock<IHolidayService>();
                    mockHolidayService.Setup(x => x.IsBusinessDayAsync(It.IsAny<DateTime>()))
                                     .ReturnsAsync(true);
                    
                    // Remove existing service if present
                    var holidayDescriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(IHolidayService));
                    if (holidayDescriptor != null)
                        services.Remove(holidayDescriptor);
                    
                    services.AddSingleton(mockHolidayService.Object);
                });
            });

            _client = _factory.CreateClient();
        }

        protected async Task<string> GetAuthTokenAsync()
        {
            var loginRequest = new
            {
                Username = "admin",
                Password = "123456"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/v1.0/auth/login", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            
            return tokenResponse.GetProperty("token").GetString()!;
        }

        protected void SetAuthToken(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected static StringContent CreateJsonContent(object obj)
        {
            var json = JsonSerializer.Serialize(obj);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        protected static async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })!;
        }
    }
}
