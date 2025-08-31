using BMPTec.ChuBank.Api.Data;
using BMPTec.ChuBank.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BMPTec.ChuBank.Api.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AppDbContext _db;
        public AccountRepository(AppDbContext db) => _db = db;

        public async Task<Account> CreateAsync(Account account)
        {
            _db.Accounts.Add(account);
            await _db.SaveChangesAsync();
            return account;
        }

        public async Task<Account?> GetByIdAsync(Guid id)
        {
            return await _db.Accounts.FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task<Account?> GetByCpfAsync(string cpf)
        {
            return await _db.Accounts.FirstOrDefaultAsync(a => a.CPF == cpf);
        }
        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            return await _db.Accounts.ToListAsync();
        }
        public async Task UpdateAsync(Account account)
        {
            _db.Accounts.Update(account);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Transfer>> GetTransfersAsync(Guid accountId, DateTime startDate, DateTime endDate)
        {
            return await _db.Transfers.AsNoTracking()
                .Where(t => (t.FromAccountId == accountId || t.ToAccountId == accountId)
                             && t.CreatedAt >= startDate.ToUniversalTime() && t.CreatedAt <= endDate.ToUniversalTime())
                .ToListAsync();
        }
    }
}
