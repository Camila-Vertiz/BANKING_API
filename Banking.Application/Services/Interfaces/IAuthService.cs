using Banking.Application.Requests.Auth;
using Banking.Application.Responses;

namespace Banking.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<UserResponse?> GetByIdAsync(Guid id);

        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
