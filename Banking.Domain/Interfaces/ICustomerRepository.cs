using Banking.Domain.Entities;

namespace Banking.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetCustomerByIdAsync(Guid Id);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
    }
}
