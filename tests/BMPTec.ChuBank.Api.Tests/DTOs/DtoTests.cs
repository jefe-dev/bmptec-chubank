using BMPTec.ChuBank.Api.DTOs;

namespace BMPTec.ChuBank.Api.Tests.DTOs
{
    public class DtoTests
    {
        [Fact]
        public void AccountCreateDto_ShouldCreateWithCorrectProperties()
        {
            // Arrange
            var name = "João Silva";
            var cpf = "12345678909";
            var balance = 1000m;

            // Act
            var dto = new AccountCreateDto(name, cpf, balance);

            // Assert
            Assert.Equal(name, dto.Name);
            Assert.Equal(cpf, dto.CPF);
            Assert.Equal(balance, dto.Balance);
        }

        [Fact]
        public void TransferCreateDto_ShouldCreateWithCorrectProperties()
        {
            // Arrange
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            var amount = 500m;

            // Act
            var dto = new TransferCreateDto(fromAccountId, toAccountId, amount);

            // Assert
            Assert.Equal(fromAccountId, dto.FromAccountId);
            Assert.Equal(toAccountId, dto.ToAccountId);
            Assert.Equal(amount, dto.Amount);
        }

        [Fact]
        public void StatementQueryDto_ShouldCreateWithCorrectProperties()
        {
            // Arrange
            var from = DateTime.UtcNow.AddDays(-30);
            var to = DateTime.UtcNow;

            // Act
            var dto = new StatementQueryDto(from, to);

            // Assert
            Assert.Equal(from, dto.From);
            Assert.Equal(to, dto.To);
        }

        [Theory]
        [InlineData("João", "111", 100)]
        [InlineData("Maria", "222", 200)]
        public void AccountCreateDto_WithDifferentValues_ShouldWork(string name, string cpf, decimal balance)
        {
            // Act
            var dto = new AccountCreateDto(name, cpf, balance);

            // Assert
            Assert.Equal(name, dto.Name);
            Assert.Equal(cpf, dto.CPF);
            Assert.Equal(balance, dto.Balance);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(100)]
        [InlineData(1000.99)]
        public void TransferCreateDto_WithDifferentAmounts_ShouldWork(decimal amount)
        {
            // Arrange
            var fromId = Guid.NewGuid();
            var toId = Guid.NewGuid();

            // Act
            var dto = new TransferCreateDto(fromId, toId, amount);

            // Assert
            Assert.Equal(amount, dto.Amount);
            Assert.Equal(fromId, dto.FromAccountId);
            Assert.Equal(toId, dto.ToAccountId);
        }

        [Fact]
        public void StatementQueryDto_WithDateRange_ShouldCalculateCorrectSpan()
        {
            // Arrange
            var from = new DateTime(2024, 1, 1);
            var to = new DateTime(2024, 1, 31);

            // Act
            var dto = new StatementQueryDto(from, to);

            // Assert
            Assert.Equal(from, dto.From);
            Assert.Equal(to, dto.To);
            Assert.True(dto.To > dto.From);
            Assert.Equal(30, (dto.To - dto.From).Days);
        }
    }
}
