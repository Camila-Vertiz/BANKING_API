namespace Banking.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        ICustomerRepository Customers { get; }
        IBankAccountRepository BankAccounts { get; }
        ITransactionRepository Transactions { get; }
        Task<int> SaveChangesAsync();
    }
}
