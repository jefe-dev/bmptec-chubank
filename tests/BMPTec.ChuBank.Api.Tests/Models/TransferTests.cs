using BMPTec.ChuBank.Api.Models;

namespace BMPTec.ChuBank.Api.Tests.Models
{
    public class TransferTests
    {
        [Fact]
        public void Transfer_ShouldInitializeWithDefaultValues()
        {
            // Act
            var transfer = new Transfer();

            // Assert
            Assert.Equal(Guid.Empty, transfer.Id);
            Assert.Equal(Guid.Empty, transfer.FromAccountId);
            Assert.Equal(Guid.Empty, transfer.ToAccountId);
            Assert.Equal(0m, transfer.Amount);
            Assert.True(transfer.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Transfer_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var fromAccountId = Guid.NewGuid();
            var toAccountId = Guid.NewGuid();
            var amount = 250.75m;

            // Act
            var transfer = new Transfer
            {
                Id = id,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                Amount = amount
            };

            // Assert
            Assert.Equal(id, transfer.Id);
            Assert.Equal(fromAccountId, transfer.FromAccountId);
            Assert.Equal(toAccountId, transfer.ToAccountId);
            Assert.Equal(amount, transfer.Amount);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(100)]
        [InlineData(1000.99)]
        [InlineData(999999.99)]
        public void Transfer_AmountProperty_ShouldAcceptPositiveValues(decimal amount)
        {
            // Arrange & Act
            var transfer = new Transfer { Amount = amount };

            // Assert
            Assert.Equal(amount, transfer.Amount);
        }

        [Fact]
        public void Transfer_ShouldAllowDifferentAccountIds()
        {
            // Arrange
            var fromId = Guid.NewGuid();
            var toId = Guid.NewGuid();

            // Act
            var transfer = new Transfer
            {
                FromAccountId = fromId,
                ToAccountId = toId
            };

            // Assert
            Assert.NotEqual(transfer.FromAccountId, transfer.ToAccountId);
            Assert.Equal(fromId, transfer.FromAccountId);
            Assert.Equal(toId, transfer.ToAccountId);
        }
    }
}
