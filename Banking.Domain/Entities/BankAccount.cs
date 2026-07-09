using Banking.Domain.Enums;

namespace Banking.Domain.Entities
{
    public class BankAccount
    {
        private readonly List<Transaction> _transactions = new();
        public Guid Id { get; private set; }
        public string Number { get; private set; } = null!;
        public decimal Balance { get; private set; }
        public CurrencyEnum Currency { get; private set; }
        public Guid CustomerId { get; private set; }
        public BankAccountStatusEnum Status { get; private set; }
        public DateTimeOffset CreatedAtUtc { get; private set; }
        public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

        private BankAccount() { }
        public BankAccount(string number, Guid customerId, decimal initialBalance = 0, CurrencyEnum currency = CurrencyEnum.Pen)
        {
            if (initialBalance < 0)
                throw new InvalidOperationException("Initial balance cannot be negative.");

            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Account number is required.", nameof(number));

            if (customerId == Guid.Empty)
                throw new ArgumentException("Customer id is required.", nameof(customerId));

            Id = Guid.NewGuid();
            Number = number;
            CustomerId = customerId;
            Balance = initialBalance;
            Status = BankAccountStatusEnum.Active;
            CreatedAtUtc = DateTimeOffset.UtcNow;
            Currency = currency;
        }
        private void EnsureAccountIsActive()
        {
            if (Status != BankAccountStatusEnum.Active)
                throw new InvalidOperationException("Bank account is not active.");
        }

        public void Credit(decimal amount)
        {
            EnsureAccountIsActive();

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            Balance += amount;
        }

        public void Debit(decimal amount)
        {
            EnsureAccountIsActive();
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));

            if (Balance < amount)
                throw new InvalidOperationException("Insufficient funds.");

            Balance -= amount;
        }

        public void CloseAccount()
        {
            if (Status == BankAccountStatusEnum.Closed)
                throw new InvalidOperationException("Account is already closed.");
            if (Balance > 0)
                throw new InvalidOperationException("Cannot close account with a positive balance. Please withdraw the remaining balance first.");
            Status = BankAccountStatusEnum.Closed;
        }

        public void BlockAccount()
        {
            if (Status == BankAccountStatusEnum.Closed)
                throw new InvalidOperationException("Cannot block a closed account.");

            Status = BankAccountStatusEnum.Blocked;
        }

        public void ActivateAccount()
        {
            if (Status == BankAccountStatusEnum.Closed)
                throw new InvalidOperationException("Cannot activate a closed account.");
            Status = BankAccountStatusEnum.Active;
        }
    }
}
