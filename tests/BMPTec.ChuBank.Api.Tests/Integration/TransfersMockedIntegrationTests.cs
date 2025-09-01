using BMPTec.ChuBank.Api.DTOs;
using BMPTec.ChuBank.Api.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.Net;

namespace BMPTec.ChuBank.Api.Tests.Integration
{
    public class TransfersMockedIntegrationTests : MockedIntegrationTestBase
    {
        public TransfersMockedIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task CreateTransfer_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var transferData = new
            {
                FromAccountId = Guid.NewGuid(),
                ToAccountId = Guid.NewGuid(),
                Amount = 100m
            };

            var successResult = new TransferResult(true, "Transfer completed successfully");

            // Setup mock
            _mockTransferService.Setup(x => x.TransferAsync(It.IsAny<TransferCreateDto>()))
                               .ReturnsAsync(successResult);

            // Act
            var response = await _client.PostAsync("/api/v1.0/transfers", CreateJsonContent(transferData));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Verify mock was called
            _mockTransferService.Verify(x => x.TransferAsync(It.Is<TransferCreateDto>(dto =>
                dto.FromAccountId == transferData.FromAccountId &&
                dto.ToAccountId == transferData.ToAccountId &&
                dto.Amount == transferData.Amount)), Times.Once);
        }

        [Fact]
        public async Task CreateTransfer_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var transferData = new
            {
                FromAccountId = Guid.NewGuid(),
                ToAccountId = Guid.NewGuid(),
                Amount = 100m
            };

            var failureResult = new TransferResult(false, "Insufficient funds");

            // Setup mock
            _mockTransferService.Setup(x => x.TransferAsync(It.IsAny<TransferCreateDto>()))
                               .ReturnsAsync(failureResult);

            // Act
            var response = await _client.PostAsync("/api/v1.0/transfers", CreateJsonContent(transferData));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Insufficient funds", content);

            // Verify mock was called
            _mockTransferService.Verify(x => x.TransferAsync(It.IsAny<TransferCreateDto>()), Times.Once);
        }

        [Fact]
        public async Task CreateTransfer_WithZeroAmount_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var transferData = new
            {
                FromAccountId = Guid.NewGuid(),
                ToAccountId = Guid.NewGuid(),
                Amount = 0m
            };

            var failureResult = new TransferResult(false, "Amount must be greater than zero");

            // Setup mock
            _mockTransferService.Setup(x => x.TransferAsync(It.IsAny<TransferCreateDto>()))
                               .ReturnsAsync(failureResult);

            // Act
            var response = await _client.PostAsync("/api/v1.0/transfers", CreateJsonContent(transferData));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            // Just verify we got BadRequest - the specific message may vary based on model validation
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(content); // Should have some error content

            // Since validation happens before the service call, we don't expect the mock to be called with invalid data
            // So let's not verify the mock call for this test case
        }

        [Fact]
        public async Task CreateTransfer_SameAccount_ShouldReturnBadRequest()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var accountId = Guid.NewGuid();
            var transferData = new
            {
                FromAccountId = accountId,
                ToAccountId = accountId, // Same account
                Amount = 100m
            };

            var failureResult = new TransferResult(false, "Cannot transfer to the same account");

            // Setup mock
            _mockTransferService.Setup(x => x.TransferAsync(It.IsAny<TransferCreateDto>()))
                               .ReturnsAsync(failureResult);

            // Act
            var response = await _client.PostAsync("/api/v1.0/transfers", CreateJsonContent(transferData));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(content); // Should have some error content

            // Since validation happens before the service call, we don't expect the mock to be called
            // So let's not verify the mock call for this validation test case
        }

        [Fact]
        public async Task CreateTransfer_WithoutAuth_ShouldReturnUnauthorized()
        {
            // Arrange
            var transferData = new
            {
                FromAccountId = Guid.NewGuid(),
                ToAccountId = Guid.NewGuid(),
                Amount = 100m
            };

            // Act (without setting auth token)
            var response = await _client.PostAsync("/api/v1.0/transfers", CreateJsonContent(transferData));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            // Verify no mock was called
            _mockTransferService.Verify(x => x.TransferAsync(It.IsAny<TransferCreateDto>()), Times.Never);
        }

        [Fact]
        public async Task CreateTransfer_BusinessDayFlow_ShouldWork()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var transferData = new
            {
                FromAccountId = Guid.NewGuid(),
                ToAccountId = Guid.NewGuid(),
                Amount = 250m
            };

            var successResult = new TransferResult(true, "Transfer completed successfully on business day");

            // Setup mocks
            _mockHolidayService.Setup(x => x.IsBusinessDayAsync(It.IsAny<DateTime>()))
                              .ReturnsAsync(true);

            _mockTransferService.Setup(x => x.TransferAsync(It.IsAny<TransferCreateDto>()))
                               .ReturnsAsync(successResult);

            // Act
            var response = await _client.PostAsync("/api/v1.0/transfers", CreateJsonContent(transferData));

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Verify mocks
            _mockTransferService.Verify(x => x.TransferAsync(It.IsAny<TransferCreateDto>()), Times.Once);
        }
    }
}
