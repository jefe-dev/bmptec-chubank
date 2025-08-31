using BMPTec.ChuBank.Api.Models;

namespace BMPTec.ChuBank.Api.Repositories
{
    public interface IAccountRepository
    {
        Task<Account> CreateAsync(Account account);
        Task<Account?> GetByIdAsync(Guid id);
        Task<Account?> GetByCpfAsync(string cpf);
        Task<IEnumerable<Account>> GetAllAsync();
        Task UpdateAsync(Account account);
        Task<IEnumerable<Transfer>> GetTransfersAsync(Guid accountId, DateTime from, DateTime to);
    }
}
