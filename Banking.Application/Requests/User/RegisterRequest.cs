
using Banking.Domain.Enums;

namespace Banking.Application.Requests.User
{
    public class RegisterRequest
    {
        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public UserRoleEnum Role { get; set; }
    }
}
