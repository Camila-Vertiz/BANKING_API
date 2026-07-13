using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using Banking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Banking.Infrastructure.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly BankingDbContext _context;

        public BankAccountRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(BankAccount bankAccount)
        {
            await _context.BankAccounts.AddAsync(bankAccount);
        }

        public async Task<IEnumerable<BankAccount>> GetAllAsync()
        {
            return await _context.BankAccounts
                .ToListAsync();
        }

        public async Task<IEnumerable<BankAccount>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.BankAccounts
                .Where(x => x.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<BankAccount?> GetByIdAsync(Guid id)
        {
            return await _context.BankAccounts
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BankAccount?> GetByNumberAsync(string number)
        {
            return await _context.BankAccounts
                .FirstOrDefaultAsync(x => x.Number == number);
        }

        public Task UpdateAsync(BankAccount bankAccount)
        {
            _context.BankAccounts.Update(bankAccount);
            return Task.CompletedTask;
        }

    }
}
