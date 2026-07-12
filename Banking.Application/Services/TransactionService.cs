using Banking.Application.Requests.Transaction;
using Banking.Application.Responses;
using Banking.Application.Security.Interfaces;
using Banking.Application.Services.Interfaces;
using Banking.Domain.Entities;
using Banking.Domain.Enums;
using Banking.Domain.Interfaces;
using FluentValidation;

namespace Banking.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IValidator<TransferRequest> _transferValidator;
        private readonly IValidator<DepositRequest> _depositValidator;
        private readonly IUnitOfWork _unitOfWork;


        public TransactionService(
            IBankAccountRepository bankAccountRepository,
            ITransactionRepository transactionRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            ICustomerRepository customerRepository,
            IValidator<TransferRequest> transferValidator,
            IValidator<DepositRequest> depositValidator)
        {
            _bankAccountRepository = bankAccountRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _customerRepository = customerRepository;
            _transferValidator = transferValidator;
            _depositValidator = depositValidator;
        }


        public async Task<IEnumerable<TransactionResponse>> TransferAsync(
            TransferRequest request)
        {
            var validationResult = await _transferValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            if (request.FromAccountId == request.ToAccountId)
                throw new InvalidOperationException(
                    "Cannot transfer to the same account.");


            var fromAccount = await _bankAccountRepository
                .GetByIdAsync(request.FromAccountId);

            if (fromAccount is null)
                throw new KeyNotFoundException(
                    "Source account not found.");


            var toAccount = await _bankAccountRepository
                .GetByIdAsync(request.ToAccountId);

            if (toAccount is null)
                throw new KeyNotFoundException(
                    "Destination account not found.");


            if (fromAccount.Currency != toAccount.Currency)
                throw new InvalidOperationException(
                    "Accounts must have the same currency.");

            if (_currentUserService.Role != "Admin")
            {
                var userId = _currentUserService.UserId;

                if (userId is null)
                    throw new UnauthorizedAccessException();


                var customer = await _customerRepository
                    .GetByIdAsync(fromAccount.CustomerId);


                if (customer is null ||
                    customer.UserId != userId)
                {
                    throw new UnauthorizedAccessException(
                        "You cannot transfer from this account.");
                }
            }

            fromAccount.Debit(request.Amount);

            toAccount.Credit(request.Amount);


            var correlationId = Guid.NewGuid();


            var debitTransaction = new Transaction(
                fromAccount.Id,
                request.Amount,
                TransactionTypeEnum.Debit,
                fromAccount.Currency,
                request.Description,
                correlationId);


            var creditTransaction = new Transaction(
                toAccount.Id,
                request.Amount,
                TransactionTypeEnum.Credit,
                toAccount.Currency,
                request.Description,
                correlationId);


            fromAccount.AddTransaction(debitTransaction);
            toAccount.AddTransaction(creditTransaction);


            await _transactionRepository.AddAsync(debitTransaction);
            await _transactionRepository.AddAsync(creditTransaction);


            await _unitOfWork.SaveChangesAsync();


            return new[]
            {
                MapToResponse(debitTransaction),
                MapToResponse(creditTransaction)
            };
        }


        public async Task<IEnumerable<TransactionResponse>> GetByAccountIdAsync(
            Guid accountId)
        {
            var account = await _bankAccountRepository
                .GetByIdAsync(accountId);

            if (account is null)
                return Enumerable.Empty<TransactionResponse>();


            if (_currentUserService.Role != "Admin")
            {
                var userId = _currentUserService.UserId;

                if (userId is null)
                    return Enumerable.Empty<TransactionResponse>();


                var customer = await _customerRepository
                    .GetByIdAsync(account.CustomerId);


                if (customer is null ||
                    customer.UserId != userId)
                {
                    return Enumerable.Empty<TransactionResponse>();
                }
            }


            var transactions = await _transactionRepository
                .GetByAccountIdAsync(accountId);


            return transactions.Select(MapToResponse);
        }

        public async Task<IEnumerable<TransactionResponse>> GetByTraceIdAsync(
            Guid traceId)
        {
            if (_currentUserService.Role != "Admin")
            {
                return Enumerable.Empty<TransactionResponse>();
            }


            var transactions = await _transactionRepository
                .GetByTraceIdAsync(traceId);


            return transactions.Select(MapToResponse);
        }

        private static TransactionResponse MapToResponse(
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

        public async Task<TransactionResponse> DepositAsync(
            DepositRequest request)
        {
            var validationResult =
                await _depositValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);


            var account = await _bankAccountRepository
                .GetByIdAsync(request.AccountId);


            if (account is null)
                throw new KeyNotFoundException(
                    "Account not found.");


            account.Credit(request.Amount);


            var transaction = new Transaction(
                account.Id,
                request.Amount,
                TransactionTypeEnum.Credit,
                account.Currency,
                request.Description,
                Guid.NewGuid());


            account.AddTransaction(transaction);


            await _transactionRepository
                .AddAsync(transaction);


            await _unitOfWork.SaveChangesAsync();


            return MapToResponse(transaction);
        }
    }
}