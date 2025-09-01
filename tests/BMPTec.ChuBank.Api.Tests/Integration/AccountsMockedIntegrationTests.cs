using BMPTec.ChuBank.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.Net;

namespace BMPTec.ChuBank.Api.Tests.Integration
{
    public class AccountsMockedIntegrationTests : MockedIntegrationTestBase
    {
        public AccountsMockedIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        [Fact]
        public async Task CreateAccount_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var accountData = new
            {
                Name = "João Silva",
                CPF = "12345678901",
                Balance = 1000m
            };

            var expectedAccount = new Account 
            { 
                Id = Guid.NewGuid(), 
                Name = accountData.Name, 
                CPF = accountData.CPF, 
                Balance = accountData.Balance 
            };

            // Setup mocks
            _mockAccountRepo.Setup(x => x.GetByCpfAsync(accountData.CPF))
                           .ReturnsAsync((Account?)null); // CPF not exists

            _mockAccountRepo.Setup(x => x.CreateAsync(It.IsAny<Account>()))
                           .ReturnsAsync(expectedAccount);

            // Act
            var response = await _client.PostAsync("/api/v1.0/accounts", CreateJsonContent(accountData));

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var account = await DeserializeResponse<Account>(response);
            Assert.Equal(expectedAccount.Name, account.Name);
            Assert.Equal(expectedAccount.CPF, account.CPF);
            Assert.Equal(expectedAccount.Balance, account.Balance);
            Assert.Equal(expectedAccount.Id, account.Id);

            // Verify mocks were called
            _mockAccountRepo.Verify(x => x.GetByCpfAsync(accountData.CPF), Times.Once);
            _mockAccountRepo.Verify(x => x.CreateAsync(It.IsAny<Account>()), Times.Once);
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
                CPF = "12345678901",
                Balance = 1000m
            };

            var existingAccount = new Account 
            { 
                Id = Guid.NewGuid(), 
                Name = "Existing User", 
                CPF = accountData.CPF, 
                Balance = 500m 
            };

            // Setup mocks
            _mockAccountRepo.Setup(x => x.GetByCpfAsync(accountData.CPF))
                           .ReturnsAsync(existingAccount); // CPF already exists

            // Act
            var response = await _client.PostAsync("/api/v1.0/accounts", CreateJsonContent(accountData));

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("CPF already registered", content);

            // Verify mocks
            _mockAccountRepo.Verify(x => x.GetByCpfAsync(accountData.CPF), Times.Once);
            _mockAccountRepo.Verify(x => x.CreateAsync(It.IsAny<Account>()), Times.Never);
        }

        [Fact]
        public async Task GetAccount_WithExistingId_ShouldReturnAccount()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var accountId = Guid.NewGuid();
            var expectedAccount = new Account 
            { 
                Id = accountId, 
                Name = "Maria Santos", 
                CPF = "98765432100", 
                Balance = 500m 
            };

            // Setup mock
            _mockAccountRepo.Setup(x => x.GetByIdAsync(accountId))
                           .ReturnsAsync(expectedAccount);

            // Act
            var response = await _client.GetAsync($"/api/v1.0/accounts/{accountId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var account = await DeserializeResponse<Account>(response);
            Assert.Equal(expectedAccount.Id, account.Id);
            Assert.Equal(expectedAccount.Name, account.Name);
            Assert.Equal(expectedAccount.CPF, account.CPF);
            Assert.Equal(expectedAccount.Balance, account.Balance);

            // Verify mock
            _mockAccountRepo.Verify(x => x.GetByIdAsync(accountId), Times.Once);
        }

        [Fact]
        public async Task GetAccount_WithNonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var nonExistentId = Guid.NewGuid();

            // Setup mock
            _mockAccountRepo.Setup(x => x.GetByIdAsync(nonExistentId))
                           .ReturnsAsync((Account?)null);

            // Act
            var response = await _client.GetAsync($"/api/v1.0/accounts/{nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            // Verify mock
            _mockAccountRepo.Verify(x => x.GetByIdAsync(nonExistentId), Times.Once);
        }

        [Fact]
        public async Task GetAllAccounts_ShouldReturnAccountsList()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            var expectedAccounts = new List<Account>
            {
                new() { Id = Guid.NewGuid(), Name = "Pedro Lima", CPF = "11111111111", Balance = 300m },
                new() { Id = Guid.NewGuid(), Name = "Ana Costa", CPF = "22222222222", Balance = 800m }
            };

            // Setup mock
            _mockAccountRepo.Setup(x => x.GetAllAsync())
                           .ReturnsAsync(expectedAccounts);

            // Act
            var response = await _client.GetAsync("/api/v1.0/accounts");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var accounts = await DeserializeResponse<List<Account>>(response);
            Assert.Equal(2, accounts.Count);
            Assert.Contains(accounts, a => a.CPF == "11111111111");
            Assert.Contains(accounts, a => a.CPF == "22222222222");

            // Verify mock
            _mockAccountRepo.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAccounts_EmptyDatabase_ShouldReturnEmptyList()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            SetAuthToken(token);

            // Setup mock to return empty list
            _mockAccountRepo.Setup(x => x.GetAllAsync())
                           .ReturnsAsync(new List<Account>());

            // Act
            var response = await _client.GetAsync("/api/v1.0/accounts");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var accounts = await DeserializeResponse<List<Account>>(response);
            Assert.Empty(accounts);

            // Verify mock
            _mockAccountRepo.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAccount_WithoutAuth_ShouldReturnUnauthorized()
        {
            // Arrange
            var accountData = new
            {
                Name = "Teste Sem Auth",
                CPF = "99999999999",
                Balance = 100m
            };

            // Act (without setting auth token)
            var response = await _client.PostAsync("/api/v1.0/accounts", CreateJsonContent(accountData));

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

            // Verify no mocks were called
            _mockAccountRepo.Verify(x => x.GetByCpfAsync(It.IsAny<string>()), Times.Never);
            _mockAccountRepo.Verify(x => x.CreateAsync(It.IsAny<Account>()), Times.Never);
        }
    }
}
