using Banking.Domain.Interfaces;

namespace Banking.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankingDbContext _context;

        public UnitOfWork(
            BankingDbContext context,
            IUserRepository users,
            ICustomerRepository customers,
            IBankAccountRepository bankAccounts,
            ITransactionRepository transactions)
        {
            _context = context;
            Users = users;
            Customers = customers;
            BankAccounts = bankAccounts;
            Transactions = transactions;
        }

        public IUserRepository Users { get; }

        public ICustomerRepository Customers { get; }

        public IBankAccountRepository BankAccounts { get; }

        public ITransactionRepository Transactions { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}