using Banking.Application.Requests.Auth;
using Banking.Application.Responses;
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
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<ICustomerRepository> _customerRepository;
        private readonly Mock<IPasswordHasher> _passwordHasher;
        private readonly Mock<IJwtTokenGenerator> _jwtTokenGenerator;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IValidator<RegisterRequest>> _registerValidator;
        private readonly Mock<IValidator<LoginRequest>> _loginValidator;


        public AuthServiceTests()
        {
            _userRepository = new();
            _customerRepository = new();
            _passwordHasher = new();
            _jwtTokenGenerator = new();
            _unitOfWork = new();

            _registerValidator = new();
            _loginValidator = new();
        }

        [Fact]
        public async Task LoginAsync_Should_Return_Token_When_Credentials_Are_Valid()
        {

            _loginValidator
                .Setup(x => x.ValidateAsync(
                    It.IsAny<LoginRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            var user = new User(
                "juan",
                "hashed-password",
                UserRoleEnum.Customer);


            _userRepository
                .Setup(x => x.GetActiveUserByUsernameAsync("juan"))
                .ReturnsAsync(user);


            _passwordHasher
                .Setup(x => x.Verify(
                    "password123",
                    "hashed-password"))
                .Returns(true);


            _jwtTokenGenerator
                .Setup(x => x.GenerateToken(user))
                .Returns(new JwtTokenResult
                {
                    Token = "jwt-token",
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                });


            var service = new AuthService(
                _userRepository.Object,
                _customerRepository.Object,
                _passwordHasher.Object,
                _unitOfWork.Object,
                _jwtTokenGenerator.Object,
                _registerValidator.Object,
                _loginValidator.Object);


            var request = new LoginRequest
            {
                UserName = "Juan",
                Password = "password123"
            };


            var result = await service.LoginAsync(request);


            result.Should()
                .NotBeNull();


            result.Token.Should()
                .Be("jwt-token");


            _jwtTokenGenerator.Verify(
                x => x.GenerateToken(user),
                Times.Once);
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_When_User_Does_Not_Exist()
        {
            _loginValidator
                .Setup(x => x.ValidateAsync(
                    It.IsAny<LoginRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepository
                .Setup(x => x.GetActiveUserByUsernameAsync("juan"))
                .ReturnsAsync((User?)null);


            var service = new AuthService(
                _userRepository.Object,
                _customerRepository.Object,
                _passwordHasher.Object,
                _unitOfWork.Object,
                _jwtTokenGenerator.Object,
                _registerValidator.Object,
                _loginValidator.Object);


            var request = new LoginRequest
            {
                UserName = "Juan",
                Password = "password123"
            };


            Func<Task> act = async () =>
                await service.LoginAsync(request);


            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid credentials.");


            _jwtTokenGenerator.Verify(
                x => x.GenerateToken(It.IsAny<User>()),
                Times.Never);
        }

        [Fact]
        public async Task LoginAsync_Should_Throw_When_Password_Is_Invalid()
        {
            _loginValidator
                .Setup(x => x.ValidateAsync(
                    It.IsAny<LoginRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());


            var user = new User(
                "juan",
                "hashed-password",
                UserRoleEnum.Customer);


            _userRepository
                .Setup(x => x.GetActiveUserByUsernameAsync("juan"))
                .ReturnsAsync(user);


            _passwordHasher
                .Setup(x => x.Verify(
                    "wrong-password",
                    "hashed-password"))
                .Returns(false);


            var service = new AuthService(
                _userRepository.Object,
                _customerRepository.Object,
                _passwordHasher.Object,
                _unitOfWork.Object,
                _jwtTokenGenerator.Object,
                _registerValidator.Object,
                _loginValidator.Object);


            var request = new LoginRequest
            {
                UserName = "Juan",
                Password = "wrong-password"
            };


            // Act

            Func<Task> act = async () =>
                await service.LoginAsync(request);


            // Assert

            await act.Should()
                .ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid credentials.");


            _jwtTokenGenerator.Verify(
                x => x.GenerateToken(It.IsAny<User>()),
                Times.Never);
        }
    }
}