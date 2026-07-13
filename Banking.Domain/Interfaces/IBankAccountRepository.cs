using Banking.Domain.Entities;

namespace Banking.Domain.Interfaces
{
    public interface IBankAccountRepository
    {
        Task<BankAccount?> GetByIdAsync(Guid id);
        Task<BankAccount?> GetByNumberAsync(string number);
        Task AddAsync(BankAccount bankAccount);
        Task UpdateAsync(BankAccount bankAccount);
        Task<IEnumerable<BankAccount>> GetByCustomerIdAsync(Guid customerId);
        Task<IEnumerable<BankAccount>> GetAllAsync();
    }
}
