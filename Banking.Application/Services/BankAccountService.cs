using Banking.Application.Requests.BankAccount;
using Banking.Application.Responses;
using Banking.Application.Services.Interfaces;
using Banking.Domain.Entities;
using Banking.Domain.Interfaces;

namespace Banking.Application.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;


        public BankAccountService(
            IBankAccountRepository bankAccountRepository,
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork)
        {
            _bankAccountRepository = bankAccountRepository;
            _customerRepository = customerRepository;
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


        public Task<BankAccountResponse?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }


        public Task<IEnumerable<BankAccountResponse>> GetByCustomerIdAsync(
            Guid customerId)
        {
            throw new NotImplementedException();
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