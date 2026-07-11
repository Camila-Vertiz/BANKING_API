using Banking.Application.Request.Customer;
using Banking.Application.Responses;

namespace Banking.Application.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);

        Task<CustomerResponse?> GetByIdAsync(Guid id);

        Task<IEnumerable<CustomerResponse>> GetAllAsync();

        Task UpdateProfileAsync(Guid id, UpdateCustomerProfileRequest request);
    }
}
