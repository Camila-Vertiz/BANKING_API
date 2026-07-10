using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using Banking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Repositories
{
    public class CustomerRepository:ICustomerRepository
    {
        private readonly BankingDbContext _context;

        public CustomerRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(Guid Id)
        {
            return await _context.Customers
                .Include(x => x.BankAccounts)
                .FirstOrDefaultAsync(x => x.Id == Id);
        }

        public Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            return Task.CompletedTask;
        }
    }
}
