using Banking.Application.Responses;
using Banking.Domain.Entities;
namespace Banking.Application.Security.Interfaces
{
    public interface IJwtTokenGenerator
    {
        JwtTokenResult GenerateToken(User user);
    }
}
