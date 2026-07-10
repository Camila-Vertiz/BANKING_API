using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using Banking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BankingDbContext _context;

        public UserRepository(BankingDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<bool> ExistsByIdAsync(Guid id)
        {
            return await _context.Users.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> ExistsByUsernameAsync(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == userName);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<User?> GetByUsernameAsync(string userName)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }
    }
}
