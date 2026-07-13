using Banking.Application.Requests.Transaction;
using Banking.Application.Security.Interfaces;
using Banking.Application.Services;
using Banking.Domain.Entities;
using Banking.Domain.Enums;
using Banking.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Banking.Tests.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<IBankAccountRepository> _bankAccountRepository;
        private readonly Mock<ITransactionRepository> _transactionRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ICurrentUserService> _currentUserService;
        private readonly Mock<ICustomerRepository> _customerRepository;
        private readonly Mock<IValidator<TransferRequest>> _transferValidator;
        private readonly Mock<IValidator<DepositRequest>> _depositValidator;


        public TransactionServiceTests()
        {
            _bankAccountRepository = new();
            _transactionRepository = new();
            _unitOfWork = new();
            _currentUserService = new();
            _customerRepository = new();
            _transferValidator = new();
            _depositValidator = new();
        }


        [Fact]
        public async Task TransferAsync_Should_Transfer_Money_Successfully()
        {

            var userId = Guid.NewGuid();

            var customerId = Guid.NewGuid();


            var fromAccount = new BankAccount(
                "1111111111",
                customerId,
                1000,
                CurrencyEnum.Pen);


            var toAccount = new BankAccount(
                "2222222222",
                Guid.NewGuid(),
                0,
                CurrencyEnum.Pen);



            var customer = new Customer(
                DocumentTypeEnum.Dni,
                "12345678",
                "Juan Perez",
                "juan@test.com",
                userId);



            _transferValidator
                .Setup(x => x.ValidateAsync(
                    It.IsAny<TransferRequest>(),
                    default))
                .ReturnsAsync(new ValidationResult());


            _bankAccountRepository
                .Setup(x => x.GetByIdAsync(fromAccount.Id))
                .ReturnsAsync(fromAccount);


            _bankAccountRepository
                .Setup(x => x.GetByIdAsync(toAccount.Id))
                .ReturnsAsync(toAccount);


            _currentUserService
                .Setup(x => x.Role)
                .Returns("Customer");


            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);


            _customerRepository
                .Setup(x => x.GetByIdAsync(customerId))
                .ReturnsAsync(customer);


            _unitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);



            var service = new TransactionService(
                _bankAccountRepository.Object,
                _transactionRepository.Object,
                _unitOfWork.Object,
                _currentUserService.Object,
                _customerRepository.Object,
                _transferValidator.Object,
                _depositValidator.Object);



            var request = new TransferRequest
            {
                FromAccountId = fromAccount.Id,
                ToAccountId = toAccount.Id,
                Amount = 300,
                Description = "Test transfer"
            };


            var result = await service.TransferAsync(request);


            result.Should()
                .HaveCount(2);


            fromAccount.Balance.Should()
                .Be(700);


            toAccount.Balance.Should()
                .Be(300);


            result.First().TraceId
                .Should()
                .Be(result.Last().TraceId);


            _transactionRepository.Verify(
                x => x.AddAsync(It.IsAny<Transaction>()),
                Times.Exactly(2));


            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task TransferAsync_Should_Throw_When_Balance_Is_Insufficient()
        {
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();


            var fromAccount = new BankAccount(
                "1111111111",
                customerId,
                100,
                CurrencyEnum.Pen);


            var toAccount = new BankAccount(
                "2222222222",
                Guid.NewGuid(),
                0,
                CurrencyEnum.Pen);



            var customer = new Customer(
                DocumentTypeEnum.Dni,
                "12345678",
                "Juan Perez",
                "juan@test.com",
                userId);



            _transferValidator
                .Setup(x => x.ValidateAsync(
                    It.IsAny<TransferRequest>(),
                    default))
                .ReturnsAsync(new ValidationResult());


            _bankAccountRepository
                .Setup(x => x.GetByIdAsync(fromAccount.Id))
                .ReturnsAsync(fromAccount);


            _bankAccountRepository
                .Setup(x => x.GetByIdAsync(toAccount.Id))
                .ReturnsAsync(toAccount);


            _currentUserService
                .Setup(x => x.Role)
                .Returns("Customer");


            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);


            _customerRepository
                .Setup(x => x.GetByIdAsync(customerId))
                .ReturnsAsync(customer);



            var service = new TransactionService(
                _bankAccountRepository.Object,
                _transactionRepository.Object,
                _unitOfWork.Object,
                _currentUserService.Object,
                _customerRepository.Object,
                _transferValidator.Object,
                _depositValidator.Object);



            var request = new TransferRequest
            {
                FromAccountId = fromAccount.Id,
                ToAccountId = toAccount.Id,
                Amount = 300,
                Description = "Insufficient funds test"
            };


            Func<Task> act = async () =>
                await service.TransferAsync(request);



            await act.Should()
                .ThrowAsync<InvalidOperationException>()
                .WithMessage("Insufficient funds.");


            _transactionRepository.Verify(
                x => x.AddAsync(It.IsAny<Transaction>()),
                Times.Never);
        }

        [Fact]
        public async Task TransferAsync_Should_Throw_When_Source_Account_Does_Not_Exist()
        {
            // Arrange

            var request = new TransferRequest
            {
                FromAccountId = Guid.NewGuid(),
                ToAccountId = Guid.NewGuid(),
                Amount = 100,
                Description = "Invalid source account"
            };


            _transferValidator
                .Setup(x => x.ValidateAsync(
                    It.IsAny<TransferRequest>(),
                    default))
                .ReturnsAsync(new ValidationResult());


            _bankAccountRepository
                .Setup(x => x.GetByIdAsync(request.FromAccountId))
                .ReturnsAsync((BankAccount?)null);


            var service = new TransactionService(
                _bankAccountRepository.Object,
                _transactionRepository.Object,
                _unitOfWork.Object,
                _currentUserService.Object,
                _customerRepository.Object,
                _transferValidator.Object,
                _depositValidator.Object);



            Func<Task> act = async () =>
                await service.TransferAsync(request);


            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Source account not found.");


            _transactionRepository.Verify(
                x => x.AddAsync(It.IsAny<Transaction>()),
                Times.Never);
        }

        [Fact]
        public async Task TransferAsync_Should_Throw_When_Destination_Account_Does_Not_Exist()
        {
            var userId = Guid.NewGuid();
            var customerId = Guid.NewGuid();


            var fromAccount = new BankAccount(
                "1111111111",
                customerId,
                1000,
                CurrencyEnum.Pen);



            var customer = new Customer(
                DocumentTypeEnum.Dni,
                "12345678",
                "Juan Perez",
                "juan@test.com",
                userId);



            var request = new TransferRequest
            {
                FromAccountId = fromAccount.Id,
                ToAccountId = Guid.NewGuid(),
                Amount = 100,
                Description = "Invalid destination account"
            };


            _transferValidator
                .Setup(x => x.ValidateAsync(
                    It.IsAny<TransferRequest>(),
                    default))
                .ReturnsAsync(new ValidationResult());


            _bankAccountRepository
                .Setup(x => x.GetByIdAsync(fromAccount.Id))
                .ReturnsAsync(fromAccount);


            _bankAccountRepository
                .Setup(x => x.GetByIdAsync(request.ToAccountId))
                .ReturnsAsync((BankAccount?)null);


            _currentUserService
                .Setup(x => x.Role)
                .Returns("Customer");


            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);


            _customerRepository
                .Setup(x => x.GetByIdAsync(customerId))
                .ReturnsAsync(customer);



            var service = new TransactionService(
                _bankAccountRepository.Object,
                _transactionRepository.Object,
                _unitOfWork.Object,
                _currentUserService.Object,
                _customerRepository.Object,
                _transferValidator.Object,
                _depositValidator.Object);



            Func<Task> act = async () =>
                await service.TransferAsync(request);



            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Destination account not found.");


            _transactionRepository.Verify(
                x => x.AddAsync(It.IsAny<Transaction>()),
                Times.Never);
        }

        [Fact]
        public async Task TransferAsync_Should_Throw_When_Amount_Is_Invalid()
        {
            // Arrange

            var request = new TransferRequest
            {
                FromAccountId = Guid.NewGuid(),
                ToAccountId = Guid.NewGuid(),
                Amount = 0,
                Description = "Invalid amount"
            };


            _transferValidator
                .Setup(x => x.ValidateAsync(
                    It.IsAny<TransferRequest>(),
                    default))
                .ReturnsAsync(
                    new ValidationResult(
                        new[]
                        {
                    new ValidationFailure(
                        "Amount",
                        "Amount must be greater than zero.")
                        }));


            var service = new TransactionService(
                _bankAccountRepository.Object,
                _transactionRepository.Object,
                _unitOfWork.Object,
                _currentUserService.Object,
                _customerRepository.Object,
                _transferValidator.Object,
                _depositValidator.Object);



            // Act

            Func<Task> act = async () =>
                await service.TransferAsync(request);



            // Assert

            await act.Should()
                .ThrowAsync<ValidationException>();


            _bankAccountRepository.Verify(
                x => x.GetByIdAsync(It.IsAny<Guid>()),
                Times.Never);


            _transactionRepository.Verify(
                x => x.AddAsync(It.IsAny<Transaction>()),
                Times.Never);


            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Never);
        }
    }
}