using BMPTec.ChuBank.Api.Models;

namespace BMPTec.ChuBank.Api.Tests.Models
{
    public class AccountTests
    {
        [Fact]
        public void Account_ShouldInitializeWithDefaultValues()
        {
            // Act
            var account = new Account();

            // Assert
            Assert.Equal(Guid.Empty, account.Id);
            Assert.Null(account.Name);
            Assert.Null(account.CPF);
            Assert.Equal(0m, account.Balance);
            Assert.True(account.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Account_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "João Silva";
            var cpf = "12345678909";
            var balance = 1000.50m;

            // Act
            var account = new Account
            {
                Id = id,
                Name = name,
                CPF = cpf,
                Balance = balance
            };

            // Assert
            Assert.Equal(id, account.Id);
            Assert.Equal(name, account.Name);
            Assert.Equal(cpf, account.CPF);
            Assert.Equal(balance, account.Balance);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100.50)]
        [InlineData(-50.25)]
        public void Account_BalanceProperty_ShouldAcceptVariousValues(decimal balance)
        {
            // Arrange & Act
            var account = new Account { Balance = balance };

            // Assert
            Assert.Equal(balance, account.Balance);
        }
    }
}