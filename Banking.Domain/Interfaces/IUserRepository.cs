using Banking.Domain.Entities;

namespace Banking.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        Task<bool> ExistsByIdAsync(Guid id);
        Task<bool> ExistsByUsernameAsync(string username);
    }
}
