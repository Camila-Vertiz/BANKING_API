using Banking.Domain.Entities;

namespace Banking.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId);
    }
}
