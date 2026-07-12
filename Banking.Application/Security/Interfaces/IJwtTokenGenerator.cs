using Banking.Domain.Entities;
namespace Banking.Application.Security.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
