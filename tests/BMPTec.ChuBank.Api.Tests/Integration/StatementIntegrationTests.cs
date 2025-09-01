using BMPTec.ChuBank.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.Net;

namespace BMPTec.ChuBank.Api.Tests.Integration
{
    public class StatementIntegrationTests : IntegrationTestBase
    {
        public StatementIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task GetStatement_WithValidDateRange_ShouldReturnTransfers()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var accountId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;

            var expectedTransfers = new List<Transfer>
            {
                new() 
                { 
                    Id = Guid.NewGuid(), 
                    FromAccountId = accountId, 
                    ToAccountId = Guid.NewGuid(), 
                    Amount = 100m, 
                    CreatedAt = DateTime.UtcNow.AddDays(-15) 
                },
                new() 
                { 
                    Id = Guid.NewGuid(), 
                    FromAccountId = Guid.NewGuid(), 
                    ToAccountId = accountId, 
                    Amount = 50m, 
                    CreatedAt = DateTime.UtcNow.AddDays(-10) 
                }
            };

            // Setup mock - be more flexible with date matching
            _mockAccountRepo.Setup(x => x.GetTransfersAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ReturnsAsync(expectedTransfers);

            // Act
            var response = await _client.GetAsync($"/api/v1.0/statement?accountId={accountId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var transfers = await DeserializeResponse<List<Transfer>>(response);
            Assert.Equal(2, transfers.Count);
            Assert.Contains(transfers, t => t.Amount == 100m);
            Assert.Contains(transfers, t => t.Amount == 50m);

            // Verify mock was called
            _mockAccountRepo.Verify(x => x.GetTransfersAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task GetStatement_WithInvalidDateRange_ShouldReturnOk()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var accountId = Guid.NewGuid();
            var startDate = DateTime.UtcNow; // Start date after end date
            var endDate = DateTime.UtcNow.AddDays(-30);

            // Setup mock to return empty list for invalid date range
            _mockAccountRepo.Setup(x => x.GetTransfersAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ReturnsAsync(new List<Transfer>());

            // Act
            var response = await _client.GetAsync($"/api/v1.0/statement?accountId={accountId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var transfers = await DeserializeResponse<List<Transfer>>(response);
            Assert.Empty(transfers);

            // Verify mock was called (no validation in controller anymore)
            _mockAccountRepo.Verify(x => x.GetTransfersAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task GetStatement_EmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var accountId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;

            // Setup mock to return empty list
            _mockAccountRepo.Setup(x => x.GetTransfersAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ReturnsAsync(new List<Transfer>());

            // Act
            var response = await _client.GetAsync($"/api/v1.0/statement?accountId={accountId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var transfers = await DeserializeResponse<List<Transfer>>(response);
            Assert.Empty(transfers);

            // Verify mock
            _mockAccountRepo.Verify(x => x.GetTransfersAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task GetStatement_WithoutAuth_ShouldReturnUnauthorized()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;

            // Act (without setting auth token)
            var response = await _client.GetAsync($"/api/v1.0/statement?accountId={accountId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            // Verify no mock was called
            _mockAccountRepo.Verify(x => x.GetTransfersAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task GetStatement_MonthlyReport_ShouldWork()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var accountId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var monthlyTransfers = new List<Transfer>
            {
                new() 
                { 
                    Id = Guid.NewGuid(), 
                    FromAccountId = accountId, 
                    ToAccountId = Guid.NewGuid(), 
                    Amount = 1000m, 
                    CreatedAt = new DateTime(2024, 1, 15) 
                },
                new() 
                { 
                    Id = Guid.NewGuid(), 
                    FromAccountId = Guid.NewGuid(), 
                    ToAccountId = accountId, 
                    Amount = 2000m, 
                    CreatedAt = new DateTime(2024, 1, 20) 
                },
                new() 
                { 
                    Id = Guid.NewGuid(), 
                    FromAccountId = accountId, 
                    ToAccountId = Guid.NewGuid(), 
                    Amount = 500m, 
                    CreatedAt = new DateTime(2024, 1, 25) 
                }
            };

            // Setup mock
            _mockAccountRepo.Setup(x => x.GetTransfersAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                           .ReturnsAsync(monthlyTransfers);

            // Act
            var response = await _client.GetAsync($"/api/v1.0/statement?accountId={accountId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var transfers = await DeserializeResponse<List<Transfer>>(response);
            Assert.Equal(3, transfers.Count);
            
            // Verify we have both incoming and outgoing transfers
            Assert.Contains(transfers, t => t.FromAccountId == accountId && t.Amount == 1000m);
            Assert.Contains(transfers, t => t.ToAccountId == accountId && t.Amount == 2000m);
            Assert.Contains(transfers, t => t.FromAccountId == accountId && t.Amount == 500m);

            // Verify mock
            _mockAccountRepo.Verify(x => x.GetTransfersAsync(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }
    }
}
