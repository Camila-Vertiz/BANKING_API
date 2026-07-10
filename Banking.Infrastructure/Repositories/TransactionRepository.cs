using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using Banking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankingDbContext _context;

        public TransactionRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
        }

        public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.Transactions
                .AsNoTracking()
                .Where(x => x.AccountId == accountId)
                .OrderByDescending(x => x.DateUtc)
                .ToListAsync();
        }
    }
}
