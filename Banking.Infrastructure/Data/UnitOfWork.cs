using Banking.Domain.Interfaces;

namespace Banking.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BankingDbContext _context;

        public UnitOfWork(
            BankingDbContext context)
        {
            _context = context;
        }


        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}