namespace Banking.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        IBankAccountRepository BankAccountRepository { get; }
        ITransactionRepository TransactionRepository { get; }
        Task<int> SaveChangesAsync();
    }
}
