using BMPTec.ChuBank.Api.Models;
using BMPTec.ChuBank.Api.Repositories;
using BMPTec.ChuBank.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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
        protected readonly Mock<IAccountRepository> _mockAccountRepo;
        protected readonly Mock<ITransferService> _mockTransferService;
        protected readonly Mock<IHolidayService> _mockHolidayService;

        public IntegrationTestBase(WebApplicationFactory<Program> factory)
        {
            _mockAccountRepo = new Mock<IAccountRepository>();
            _mockTransferService = new Mock<ITransferService>();
            _mockHolidayService = new Mock<IHolidayService>();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove existing services
                    var accountRepoDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAccountRepository));
                    if (accountRepoDescriptor != null)
                        services.Remove(accountRepoDescriptor);

                    var transferServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ITransferService));
                    if (transferServiceDescriptor != null)
                        services.Remove(transferServiceDescriptor);

                    var holidayServiceDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IHolidayService));
                    if (holidayServiceDescriptor != null)
                        services.Remove(holidayServiceDescriptor);

                    // Add mocks
                    services.AddSingleton(_mockAccountRepo.Object);
                    services.AddSingleton(_mockTransferService.Object);
                    services.AddSingleton(_mockHolidayService.Object);
                });
            });

            _client = _factory.CreateClient();
        }

        protected async Task<string> GetAuthTokenAsync()
        {
            var loginRequest = new
            {
                Username = "admin",
                Password = "admin123"
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
