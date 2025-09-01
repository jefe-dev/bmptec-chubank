using BMPTec.ChuBank.Api.Data;
using BMPTec.ChuBank.Api.DTOs;
using BMPTec.ChuBank.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BMPTec.ChuBank.Api.Services
{
    public class TransferService : ITransferService
    {
        private readonly AppDbContext _db;
        private readonly IHolidayService _holidayService;

        public TransferService(AppDbContext db, IHolidayService holidayService)
        {
            _db = db;
            _holidayService = holidayService;
        }

        public async Task<TransferResult> TransferAsync(TransferCreateDto dto)
        {
            var date = DateTime.UtcNow.Date;
            if (!await _holidayService.IsBusinessDayAsync(date))
                return new TransferResult(false, "Transfers are allowed only on business days.");

            var from = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == dto.FromAccountId);
            var to = await _db.Accounts.FirstOrDefaultAsync(a => a.Id == dto.ToAccountId);

            if (from == null || to == null) return new TransferResult(false, "Account not found");
            if (from.Balance < dto.Amount) return new TransferResult(false, "Insufficient funds");

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                from.Balance -= dto.Amount;
                to.Balance += dto.Amount;
                var transfer = new Transfer
                {
                    FromAccountId = from.Id,
                    ToAccountId = to.Id,
                    Amount = dto.Amount,
                    CreatedAt = DateTime.UtcNow
                };
                _db.Transfers.Add(transfer);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
                return new TransferResult(true, null);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return new TransferResult(false, ex.Message);
            }
        }
    }
}
