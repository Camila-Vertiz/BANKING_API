using Banking.Domain.Entities;
using Banking.Domain.Enums;

namespace Banking.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid Id);
        Task<Customer?> GetByUserIdAsync(Guid userId);
        Task<Customer?> GetByDocumentAsync(DocumentTypeEnum documentType, string documentNumber);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        Task UpdateProfileAsync(Customer customer);
    }
}
