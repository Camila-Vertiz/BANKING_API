using Banking.Application.Security.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Banking.Infrastructure.Security
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private ClaimsPrincipal? User =>
            _httpContextAccessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                var value = User?
                            .FindFirst(ClaimTypes.NameIdentifier)?
                            .Value;

                return Guid.TryParse(value, out var id)
                    ? id
                    : null;
            }
        }

        public string? UserName => User?
            .FindFirst(ClaimTypes.Name)?.Value;

        public string? Role => User?
            .FindFirst(ClaimTypes.Role)?.Value;
    }
}
