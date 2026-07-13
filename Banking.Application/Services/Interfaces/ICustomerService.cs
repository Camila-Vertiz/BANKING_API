using Banking.Application.Request.Customer;
using Banking.Application.Requests.Customer;
using Banking.Application.Responses;
using Banking.Domain.Enums;

namespace Banking.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);

        Task<CustomerResponse?> GetByIdAsync(Guid id);
        Task<CustomerResponse?> GetByDocumentAsync(GetCustomerByDocumentRequest request);

        Task<IEnumerable<CustomerResponse>> GetAllAsync();

        Task UpdateProfileAsync(UpdateCustomerProfileRequest request);
    }
}
