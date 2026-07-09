using Banking.Domain.Entities;

namespace Banking.Domain.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetTransactionsByAccountIdAsync(Guid accountId);
    }
}
