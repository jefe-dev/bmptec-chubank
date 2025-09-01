using BMPTec.ChuBank.Api.DTOs;
using BMPTec.ChuBank.Api.Models;
using BMPTec.ChuBank.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BMPTec.ChuBank.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _repo;

        public AccountsController(IAccountRepository repo) => _repo = repo;

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AccountCreateDto dto)
        {
            var exist = await _repo.GetByCpfAsync(dto.CPF);
            if (exist != null) return Conflict("CPF already registered");

            var account = new Account { Name = dto.Name, CPF = dto.CPF, Balance = dto.Balance };
            var created = await _repo.CreateAsync(account);
            return CreatedAtAction(nameof(Get), new { id = created.Id, version = "1.0" }, created);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(Guid id)
        {
            var acc = await _repo.GetByIdAsync(id);
            if (acc == null) return NotFound();
            return Ok(acc);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var acc = await _repo.GetAllAsync();
            if (acc == null) return NotFound();
            return Ok(acc);
        }


    }
}
