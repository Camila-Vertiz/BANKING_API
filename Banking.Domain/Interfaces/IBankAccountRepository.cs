using Banking.Domain.Entities;

namespace Banking.Domain.Interfaces
{
    public interface IBankAccountRepository
    {
        Task<BankAccount?> GetBankAccountByIdAsync(Guid Id);
        Task<BankAccount?> GetBankAccountByNumberAsync(string Number);
        Task AddBankAccountAsync(BankAccount bankAccount);
        Task UpdateBankAccountAsync(BankAccount bankAccount);
    }
}
