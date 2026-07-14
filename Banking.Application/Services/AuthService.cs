using Banking.Application.Requests.Auth;
using Banking.Application.Responses;
using Banking.Application.Security.Interfaces;
using Banking.Application.Services.Interfaces;
using Banking.Domain.Entities;
using Banking.Domain.Enums;
using Banking.Domain.Interfaces;
using FluentValidation;

namespace Banking.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<RegisterRequest> _registerValidator;
        private readonly IValidator<LoginRequest> _loginValidator;

        public AuthService(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork,
            IJwtTokenGenerator jwtTokenGenerator,
            IValidator<RegisterRequest> registerValidator,
            IValidator<LoginRequest> loginValidator)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _jwtTokenGenerator = jwtTokenGenerator;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        private static UserResponse MapToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Role = user.Role.ToString(),
                CreatedAtUtc = user.CreatedAtUtc
            };
        }


        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var validationResult = await _registerValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var exists = await _userRepository.ExistsByUsernameAsync(request.UserName);

            if (exists)
            {
                throw new InvalidOperationException("Username already exists.");
            }

            var existingCustomer = await _customerRepository
                                            .GetByDocumentAsync(
                                                request.DocumentType,
                                                request.DocumentNumber);

            if (existingCustomer != null)
            {
                throw new InvalidOperationException("Document already exists.");
            }

            var hashedPassword = _passwordHasher.Hash(request.Password);

            var user = new User(
                request.UserName.ToLower(),
                hashedPassword,
                UserRoleEnum.Customer
            );

            await _userRepository.AddAsync(user);

            var customer = new Customer(
                request.DocumentType,
                request.DocumentNumber,
                request.FullName,
                request.Email,
                user.Id
            );

            await _customerRepository.AddAsync(customer);

            await _unitOfWork.SaveChangesAsync();

            return new RegisterResponse
            {
                UserId = user.Id,
                CustomerId = customer.Id,
                UserName = user.UserName,
                Role = user.Role.ToString(),
                CreatedAtUtc = user.CreatedAtUtc
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var validationResult = await _loginValidator.ValidateAsync(request);


            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var user = await _userRepository
                .GetActiveUserByUsernameAsync(
                    request.UserName.ToLower()
                );

            if (user is null)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var validPassword = _passwordHasher.Verify(request.Password, user.PasswordHash);

            if (!validPassword)
            {
                throw new UnauthorizedAccessException(
                    "Invalid credentials."
                );
            }

            var tokenResult = _jwtTokenGenerator.GenerateToken(user);


            return new AuthResponse
            {
                Token = tokenResult.Token,
                ExpiresAt = tokenResult.ExpiresAt
            };
        }

        public async Task<UserResponse?> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
                return null;

            return MapToResponse(user);
        }
    }
}
