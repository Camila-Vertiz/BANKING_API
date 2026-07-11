using Banking.Domain.Enums;

namespace Banking.Application.Request.Customer
{
    public class CreateCustomerRequest
    {
        public DocumentTypeEnum DocumentType { get; set; }
        public string DocumentNumber { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
