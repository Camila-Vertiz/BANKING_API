using Banking.Domain.Entities;

namespace Banking.Domain.Interfaces
{
    public interface IBankAccountRepository
    {
        Task<BankAccount?> GetByIdAsync(Guid Id);
        Task<BankAccount?> GetByNumberAsync(string Number);
        Task AddAsync(BankAccount bankAccount);
        Task UpdateAsync(BankAccount bankAccount);
    }
}
