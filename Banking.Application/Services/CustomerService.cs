using Banking.Application.Request.Customer;
using Banking.Application.Requests.Customer;
using Banking.Application.Responses;
using Banking.Application.Security.Interfaces;
using Banking.Application.Services.Interfaces;
using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using FluentValidation;

namespace Banking.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateCustomerRequest> _createCustomerValidator;

        public CustomerService(
            ICustomerRepository customerRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            IValidator<CreateCustomerRequest> createCustomerValidator)
        {
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
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
            IEnumerable<Customer> customers;

            if (_currentUserService.Role == "Admin")
            {
                customers = await _customerRepository.GetAllAsync();
            }
            else
            {
                var userId = _currentUserService.UserId;

                if (userId is null)
                    return Enumerable.Empty<CustomerResponse>();

                var customer = await _customerRepository.GetByUserIdAsync(userId.Value);

                if (customer is null)
                    return Enumerable.Empty<CustomerResponse>();

                customers = new List<Customer>
                {
                    customer
                };
            }

            return customers.Select(MapToResponse);
        }

        public async Task<CustomerResponse?> GetByIdAsync(Guid id)
        {
            Customer? customer;

            if (_currentUserService.Role == "Admin")
            {
                customer = await _customerRepository.GetByIdAsync(id);
            }
            else
            {
                var userId = _currentUserService.UserId;

                if (userId is null)
                    return null;

                customer = await _customerRepository.GetByUserIdAsync(userId.Value);

                if (customer?.Id != id)
                    return null;
            }

            if (customer is null)
                return null;

            return MapToResponse(customer);
        }

        public async Task<CustomerResponse?> GetByDocumentAsync(GetCustomerByDocumentRequest request)
        {
            var customer = await _customerRepository.GetByDocumentAsync(request.DocumentType, request.DocumentNumber);

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
