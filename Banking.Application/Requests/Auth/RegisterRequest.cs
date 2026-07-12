using Banking.Domain.Enums;

namespace Banking.Application.Requests.Auth
{
    public class RegisterRequest
    {
        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public DocumentTypeEnum DocumentType { get; set; }

        public string DocumentNumber { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;
    }
}
