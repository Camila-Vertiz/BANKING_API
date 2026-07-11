using Banking.Domain.Enums;

namespace Banking.Application.Responses
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public DocumentTypeEnum DocumentType { get; set; }
        public string DocumentNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTimeOffset CreatedAtUtc { get; set; }
    }
}
