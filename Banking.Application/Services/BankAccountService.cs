using Banking.Application.Requests.BankAccount;
using Banking.Application.Responses;
using Banking.Application.Security.Interfaces;
using Banking.Application.Services.Interfaces;
using Banking.Domain.Entities;
using Banking.Domain.Interfaces;

namespace Banking.Application.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;


        public BankAccountService(
            IBankAccountRepository bankAccountRepository,
            ICustomerRepository customerRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _bankAccountRepository = bankAccountRepository;
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }


        public async Task<BankAccountResponse> CreateAsync(
            CreateBankAccountRequest request)
        {
            var customer = await _customerRepository
                .GetByIdAsync(request.CustomerId);

            if (customer is null)
                throw new KeyNotFoundException("Customer not found.");


            var accountNumber = await GenerateAccountNumber();


            var bankAccount = new BankAccount(
                accountNumber,
                request.CustomerId,
                0,
                request.Currency);


            await _bankAccountRepository.AddAsync(bankAccount);

            await _unitOfWork.SaveChangesAsync();


            return MapToResponse(bankAccount);
        }


        public async Task<BankAccountResponse?> GetByIdAsync(Guid id)
        {
            var account = await _bankAccountRepository
                .GetByIdAsync(id);

            if (account is null)
                return null;


            if (_currentUserService.Role != "Admin")
            {
                var userId = _currentUserService.UserId;

                if (userId is null)
                    return null;


                var customer = await _customerRepository
                    .GetByIdAsync(account.CustomerId);


                if (customer is null ||
                    customer.UserId != userId)
                {
                    return null;
                }
            }


            return MapToResponse(account);
        }


        public async Task<IEnumerable<BankAccountResponse>> GetByCustomerIdAsync(
            Guid customerId)
        {
            var customer = await _customerRepository
                .GetByIdAsync(customerId);

            if (customer is null)
                return Enumerable.Empty<BankAccountResponse>();


            if (_currentUserService.Role != "Admin")
            {
                var userId = _currentUserService.UserId;

                if (userId is null ||
                    customer.UserId != userId)
                {
                    return Enumerable.Empty<BankAccountResponse>();
                }
            }


            var accounts = await _bankAccountRepository
                .GetByCustomerIdAsync(customerId);


            return accounts.Select(MapToResponse);
        }

        private async Task<string> GenerateAccountNumber()
        {
            string number;

            do
            {
                number = Random.Shared
                    .NextInt64(1000000000, 9999999999)
                    .ToString();

            } while (
                await _bankAccountRepository
                    .GetByNumberAsync(number) != null
            );

            return number;
        }

        private static BankAccountResponse MapToResponse(
            BankAccount bankAccount)
        {
            return new BankAccountResponse
            {
                Id = bankAccount.Id,
                Number = bankAccount.Number,
                Balance = bankAccount.Balance,
                Currency = bankAccount.Currency,
                CustomerId = bankAccount.CustomerId,
                Status = bankAccount.Status,
                CreatedAtUtc = bankAccount.CreatedAtUtc
            };
        }
    }
}