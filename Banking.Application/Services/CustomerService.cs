using Banking.Application.Request.Customer;
using Banking.Application.Responses;
using Banking.Application.Services.Interfaces;
using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using FluentValidation;

namespace Banking.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateCustomerRequest> _createCustomerValidator;

        public CustomerService(
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork,
            IValidator<CreateCustomerRequest> createCustomerValidator)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _createCustomerValidator = createCustomerValidator;
        }

        private static CustomerResponse MapToResponse(Customer customer)
        {
            return new CustomerResponse
            {
                Id = customer.Id,
                DocumentType = customer.DocumentType,
                DocumentNumber = customer.DocumentNumber,
                FullName = customer.FullName,
                Email = customer.Email,
                CreatedAtUtc = customer.CreatedAtUtc
            };
        }

        public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
        {
            var validationResult = await _createCustomerValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var customer = new Customer(
                request.DocumentType,
                request.DocumentNumber,
                request.FullName,
                request.Email
            );

            await _customerRepository.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(customer);

        }

        public async Task<IEnumerable<CustomerResponse>> GetAllAsync()
        {
            var customers = await _customerRepository.GetAllAsync();

            return customers.Select(MapToResponse);
        }

        public async Task<CustomerResponse?> GetByIdAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer is null)
                return null;

            return MapToResponse(customer);
        }

        public async Task<CustomerResponse?> GetByDocumentNumberAsync(string documentNumber)
        {
            var customer = await _customerRepository.GetByDocumentNumberAsync(documentNumber);

            if (customer is null)
                return null;

            return MapToResponse(customer);
        }

        public async Task UpdateProfileAsync(Guid id, UpdateCustomerProfileRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer is null)
                throw new KeyNotFoundException("Customer not found.");

            customer.UpdateBasicInfo(request.FullName, request.Email);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
