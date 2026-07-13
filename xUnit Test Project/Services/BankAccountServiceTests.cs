using Banking.Application.Requests.BankAccount;
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
    public class BankAccountServiceTests
    {
        private readonly Mock<IBankAccountRepository> _bankAccountRepository;
        private readonly Mock<ICustomerRepository> _customerRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<ICurrentUserService> _currentUserService;
        private readonly Mock<IValidator<CreateBankAccountRequest>> _validator;


        public BankAccountServiceTests()
        {
            _bankAccountRepository = new();
            _customerRepository = new();
            _unitOfWork = new();
            _currentUserService = new();
            _validator = new();
        }


        [Fact]
        public async Task CreateAsync_Should_Create_BankAccount_When_Request_Is_Valid()
        {
            var customerId = Guid.NewGuid();

            var customer = new Customer(
                DocumentTypeEnum.Dni,
                "12345678",
                "Juan Perez",
                "juan@test.com");


            _customerRepository
                .Setup(x => x.GetByIdAsync(customerId))
                .ReturnsAsync(customer);


            _validator
                .Setup(x => x.ValidateAsync(
                    It.IsAny<CreateBankAccountRequest>(),
                    default))
                .ReturnsAsync(new ValidationResult());


            _bankAccountRepository
                .Setup(x => x.GetByNumberAsync(It.IsAny<string>()))
                .ReturnsAsync((BankAccount?)null);


            _unitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);


            var service = new BankAccountService(
                _bankAccountRepository.Object,
                _customerRepository.Object,
                _unitOfWork.Object,
                _currentUserService.Object,
                _validator.Object);


            var request = new CreateBankAccountRequest
            {
                CustomerId = customerId,
                Currency = CurrencyEnum.Pen
            };


            var result = await service.CreateAsync(request);


            result.Should().NotBeNull();

            result.CustomerId.Should()
                .Be(customerId);

            result.Balance.Should()
                .Be(0);


            _bankAccountRepository.Verify(
                x => x.AddAsync(It.IsAny<BankAccount>()),
                Times.Once);


            _unitOfWork.Verify(
                x => x.SaveChangesAsync(),
                Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Throw_When_Customer_Does_Not_Exist()
        {
            var customerId = Guid.NewGuid();


            _customerRepository
                .Setup(x => x.GetByIdAsync(customerId))
                .ReturnsAsync((Customer?)null);


            _validator
                .Setup(x => x.ValidateAsync(
                    It.IsAny<CreateBankAccountRequest>(),
                    default))
                .ReturnsAsync(new ValidationResult());


            var service = new BankAccountService(
                _bankAccountRepository.Object,
                _customerRepository.Object,
                _unitOfWork.Object,
                _currentUserService.Object,
                _validator.Object);


            var request = new CreateBankAccountRequest
            {
                CustomerId = customerId,
                Currency = CurrencyEnum.Pen
            };


            Func<Task> act = async () =>
                await service.CreateAsync(request);


            await act.Should()
                .ThrowAsync<KeyNotFoundException>()
                .WithMessage("Customer not found.");
        }

        [Fact]
        public async Task GetBalanceAsync_Should_Throw_When_Customer_Does_Not_Own_Account()
        {
            // Arrange

            var accountId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();


            var account = new BankAccount(
                "1234567890",
                Guid.NewGuid(),
                500,
                CurrencyEnum.Pen);


            _bankAccountRepository
                .Setup(x => x.GetByIdAsync(accountId))
                .ReturnsAsync(account);


            var customer = new Customer(
                DocumentTypeEnum.Dni,
                "12345678",
                "Maria Perez",
                "maria@test.com");


            // Simulamos que la cuenta pertenece a otro usuario
            customer.LinkUser(otherUserId);


            _customerRepository
                .Setup(x => x.GetByIdAsync(account.CustomerId))
                .ReturnsAsync(customer);


            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _currentUserService
                .Setup(x => x.Role)
                .Returns("Customer");


            var service = new BankAccountService(
                _bankAccountRepository.Object,
                _customerRepository.Object,
                _unitOfWork.Object,
                _currentUserService.Object,
                _validator.Object);


            // Act

            Func<Task> act = async () =>
                await service.GetBalanceAsync(accountId);


            // Assert

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("You cannot access this account.");
        }
    }
}