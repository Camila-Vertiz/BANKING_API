using Banking.Domain.Enums;
namespace Banking.Domain.Entities
{
    public class Customer
    {
        private readonly List<BankAccount> _bankAccounts = new();
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
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new ArgumentException("Document number is required.", nameof(documentNumber));

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required.", nameof(fullName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

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
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name is required.", nameof(fullName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            FullName = fullName;
            Email = email;
        }
        public void LinkUser(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Use id is required");

            if (UserId.HasValue)
                throw new InvalidOperationException(
                    "Customer already has a linked user.");

            UserId = userId;
        }

    }
}
