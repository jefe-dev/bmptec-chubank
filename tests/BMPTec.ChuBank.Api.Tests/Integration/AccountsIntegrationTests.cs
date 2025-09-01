using System.Net;
using BMPTec.ChuBank.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Xunit;

namespace BMPTec.ChuBank.Api.Tests.Integration
{
    public class AccountsIntegrationTests : IntegrationTestBase
    {
        public AccountsIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task CreateAccount_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var accountData = new
            {
                Name = "João Silva",
                CPF = "12345678909",
                Balance = 1000m
            };

            var expectedAccount = new Account
            {
                Id = Guid.NewGuid(),
                Name = accountData.Name,
                CPF = accountData.CPF,
                Balance = accountData.Balance
            };

            _mockAccountRepo.Setup(x => x.GetByCpfAsync(accountData.CPF))
                .ReturnsAsync((Account?)null); // CPF not exists

            _mockAccountRepo.Setup(x => x.CreateAsync(It.IsAny<Account>()))
                .ReturnsAsync(expectedAccount);

            // Act
            var response = await _client.PostAsync("/api/v1.0/accounts", CreateJsonContent(accountData));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var account = await DeserializeResponse<Account>(response);
            Assert.NotNull(account);
            Assert.Equal(expectedAccount.Name, account.Name);
            Assert.Equal(expectedAccount.CPF, account.CPF);
            Assert.Equal(expectedAccount.Balance, account.Balance);

            _mockAccountRepo.Verify(x => x.GetByCpfAsync(accountData.CPF), Times.Once);
        }

        [Fact]
        public async Task CreateAccount_WithExistingCPF_ShouldReturnConflict()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var accountData = new
            {
                Name = "João Silva",
                CPF = "12345678909", // Valid CPF
                Balance = 1000m
            };

            var existingAccount = new Account
            {
                Id = Guid.NewGuid(),
                Name = "Existing User",
                CPF = accountData.CPF,
                Balance = 500m
            };

            _mockAccountRepo.Setup(x => x.GetByCpfAsync(accountData.CPF))
                .ReturnsAsync(existingAccount); // CPF already exists

            // Act
            var response = await _client.PostAsync("/api/v1.0/accounts", CreateJsonContent(accountData));

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("CPF already registered", content);

            _mockAccountRepo.Verify(x => x.GetByCpfAsync(accountData.CPF), Times.Once);
        }

        [Fact]
        public async Task GetAccount_WithValidId_ShouldReturnAccount()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var accountId = Guid.NewGuid();
            var expectedAccount = new Account
            {
                Id = accountId,
                Name = "Maria Santos",
                CPF = "98765432109", 
                Balance = 500m
            };

            _mockAccountRepo.Setup(x => x.GetByIdAsync(accountId))
                .ReturnsAsync(expectedAccount);

            // Act
            var response = await _client.GetAsync($"/api/v1.0/accounts/{accountId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var account = await DeserializeResponse<Account>(response);
            Assert.NotNull(account);
            Assert.Equal(expectedAccount.Id, account.Id);
            Assert.Equal(expectedAccount.Name, account.Name);
            Assert.Equal(expectedAccount.CPF, account.CPF);
            Assert.Equal(expectedAccount.Balance, account.Balance);

            _mockAccountRepo.Verify(x => x.GetByIdAsync(accountId), Times.Once);
        }

        [Fact]
        public async Task GetAccount_WithInvalidId_ShouldReturnNotFound()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var accountId = Guid.NewGuid();

            _mockAccountRepo.Setup(x => x.GetByIdAsync(accountId))
                .ReturnsAsync((Account?)null);

            // Act
            var response = await _client.GetAsync($"/api/v1.0/accounts/{accountId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            _mockAccountRepo.Verify(x => x.GetByIdAsync(accountId), Times.Once);
        }

        [Fact]
        public async Task GetAllAccounts_ShouldReturnAccounts()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var expectedAccounts = new List<Account>
            {
                new() { Id = Guid.NewGuid(), Name = "João Silva", CPF = "12345678909", Balance = 1000m },
                new() { Id = Guid.NewGuid(), Name = "Maria Santos", CPF = "98765432109", Balance = 500m },
            };

            _mockAccountRepo.Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedAccounts);

            // Act
            var response = await _client.GetAsync("/api/v1.0/accounts");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var accounts = await DeserializeResponse<List<Account>>(response);
            Assert.NotNull(accounts);
            Assert.Equal(expectedAccounts.Count, accounts.Count);
            Assert.Contains(accounts, a => a.CPF == "12345678909");
            Assert.Contains(accounts, a => a.CPF == "98765432109");

            _mockAccountRepo.Verify(x => x.GetAllAsync(), Times.Once);
        }
    }
}
