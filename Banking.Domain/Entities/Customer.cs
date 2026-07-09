using Banking.Domain.Enums;
namespace Banking.Domain.Entities
{
    public class Customer
    {
        private readonly List<BankAccount> _bankAccounts = new ();
        public Guid Id { get; private set; }
        public Guid? UserId { get; private set; }
        public DocumentTypeEnum DocumentType { get; private set; }
        public string DocumentNumber { get; private set; } = null!;
        public string FullName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public DateTimeOffset CreatedAtUtc { get; private set; }

        public IReadOnlyCollection<BankAccount> BankAccounts => _bankAccounts.AsReadOnly();

        private Customer() { }
        public Customer(DocumentTypeEnum documentType, string documentNumber, string fullName, string email, Guid? userId = null)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            DocumentType = documentType;
            DocumentNumber = documentNumber;
            FullName = fullName;
            Email = email;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }
        public void UpdateBasicInfo(string fullName, string email)
        {
            FullName = fullName;
            Email = email;
        }
        public void LinkUser(Guid userId)
        {
            UserId = userId;
        }

    }
}
