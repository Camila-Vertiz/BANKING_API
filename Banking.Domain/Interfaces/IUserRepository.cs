using Banking.Domain.Entities;

namespace Banking.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetActiveUserByUsernameAsync(string userName);
        Task AddAsync(User user);
        Task<bool> ExistsByIdAsync(Guid id);
        Task<bool> ExistsByUsernameAsync(string userName);
    }
}
