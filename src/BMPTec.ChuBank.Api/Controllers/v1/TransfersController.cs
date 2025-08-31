using BMPTec.ChuBank.Api.DTOs;
using BMPTec.ChuBank.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BMPTec.ChuBank.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class TransfersController : ControllerBase
    {
        private readonly ITransferService _transferService;
        public TransfersController(ITransferService transferService) => _transferService = transferService;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] TransferCreateDto dto)
        {
            var res = await _transferService.TransferAsync(dto);
            if (!res.Success) return BadRequest(new { message = res.Message });
            return Ok();
        }
    }
}
