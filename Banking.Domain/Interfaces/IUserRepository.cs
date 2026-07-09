using Banking.Domain.Entities;
using System.Security.Cryptography;

namespace Banking.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByUsermameAsync(string username);
        Task AddAsync(User user);
        Task<bool> ExistsByIdAsync(Guid id);
        Task<bool> ExistsByUsernameAsync(string username);
    }
}
