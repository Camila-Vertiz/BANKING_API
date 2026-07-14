using Banking.Application.Requests.BankAccount;
using Banking.Application.Responses;
using Banking.Application.Security.Interfaces;
using Banking.Application.Services.Interfaces;
using Banking.Domain.Entities;
using Banking.Domain.Interfaces;
using FluentValidation;

namespace Banking.Application.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IValidator<CreateBankAccountRequest> _createBankAccountValidator;
        private readonly IUnitOfWork _unitOfWork;


        public BankAccountService(
            IBankAccountRepository bankAccountRepository,
            ICustomerRepository customerRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ITransactionRepository transactionRepository,
            IValidator<CreateBankAccountRequest> createBankAccountValidator)
        {
            _bankAccountRepository = bankAccountRepository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _transactionRepository = transactionRepository;
            _createBankAccountValidator = createBankAccountValidator;
        }


        public async Task<BankAccountResponse> CreateAsync(
            CreateBankAccountRequest request)
        {
            var validationResult =
                await _createBankAccountValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);


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


        public async Task<IEnumerable<BankAccountResponse>> GetAllAsync()
        {
            var accounts = await _bankAccountRepository.GetAllAsync();

            return accounts.Select(MapToResponse);
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
                    throw new UnauthorizedAccessException(
                        "You cannot access this account.");
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
                    throw new UnauthorizedAccessException("You cannot access this customer accounts.");
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

        public async Task<BalanceResponse?> GetBalanceAsync(Guid id)
        {
            var account = await _bankAccountRepository
                .GetByIdAsync(id);

            if (account is null)
                return null;


            if (_currentUserService.Role != "Admin")
            {
                var userId = _currentUserService.UserId;

                var customer = await _customerRepository
                    .GetByIdAsync(account.CustomerId);


                if (customer is null ||
                    customer.UserId != userId)
                {
                    throw new UnauthorizedAccessException(
                        "You cannot access this account.");
                }
            }


            return new BalanceResponse
            {
                AccountId = account.Id,
                Balance = account.Balance,
                Currency = account.Currency
            };
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

        private static TransactionResponse MapTransactionToResponse(
            Transaction transaction)
        {
            return new TransactionResponse
            {
                Id = transaction.Id,
                AccountId = transaction.AccountId,
                TransactionType = transaction.TransactionType,
                Amount = transaction.Amount,
                Currency = transaction.Currency,
                DateUtc = transaction.DateUtc,
                Description = transaction.Description,
                TraceId = transaction.TraceId
            };
        }

        public async Task<IEnumerable<TransactionResponse>> GetTransactionsByAccountIdAsync(Guid id)
        {
            var account = await _bankAccountRepository
                .GetByIdAsync(id);

            if (account is null)
                return Enumerable.Empty<TransactionResponse>();


            if (_currentUserService.Role != "Admin")
            {
                var userId = _currentUserService.UserId;

                if (userId is null)
                    throw new UnauthorizedAccessException("You cannot access this account.");


                var customer = await _customerRepository
                    .GetByIdAsync(account.CustomerId);


                if (customer is null ||
                    customer.UserId != userId)
                {
                    throw new UnauthorizedAccessException("You cannot access this account.");
                }
            }


            var transactions = await _transactionRepository
                .GetByAccountIdAsync(id);


            return transactions.Select(MapTransactionToResponse);
        }
    }
}