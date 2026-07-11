using Banking.Domain.Enums;

namespace Banking.Application.Requests.Customer
{
    public class GetCustomerByDocumentRequest
    {
        public DocumentTypeEnum DocumentType { get; set; }

        public string DocumentNumber { get; set; } = null!;
    }
}
