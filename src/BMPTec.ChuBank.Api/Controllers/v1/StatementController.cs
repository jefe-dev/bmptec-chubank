using BMPTec.ChuBank.Api.DTOs;
using BMPTec.ChuBank.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BMPTec.ChuBank.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class StatementController : ControllerBase
    {
        private readonly IAccountRepository _repo;

        public StatementController(IAccountRepository repo) => _repo = repo;

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] Guid accountId, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var transfers = await _repo.GetTransfersAsync(accountId, startDate, endDate);
            return Ok(transfers);
        }
    }
}
