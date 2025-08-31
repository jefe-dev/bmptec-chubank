using BMPTec.ChuBank.Api.Controllers.v1;
using BMPTec.ChuBank.Api.DTOs;
using BMPTec.ChuBank.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

public class AccountsControllerTests
{
    [Fact]
    public async Task Create_ReturnsConflict_WhenCpfExists()
    {
        var repoMock = new Mock<IAccountRepository>();
        repoMock.Setup(r => r.GetByCpfAsync(It.IsAny<string>()))
                .ReturnsAsync(new BMPTec.ChuBank.Api.Models.Account());
        var controller = new AccountsController(repoMock.Object);

        var result = await controller.Create(new AccountCreateDto("Name","12345678901", 111111));

        Assert.IsType<ConflictObjectResult>(result);
    }
}
