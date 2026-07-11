using Banking.Domain.Entities;

namespace Banking.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid Id);
        Task<Customer?> GetByDocumentNumberAsync(string documentNumber);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        Task UpdateProfileAsync(Customer customer);
    }
}
