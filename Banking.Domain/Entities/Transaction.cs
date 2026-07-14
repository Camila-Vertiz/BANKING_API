using Banking.Domain.Enums;
namespace Banking.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid AccountId { get; private set; }
        public TransactionTypeEnum TransactionType { get; private set; }
        public decimal Amount { get; private set; }
        public CurrencyEnum Currency { get; private set; }
        public DateTimeOffset DateUtc { get; private set; }
        public string Description { get; private set; } = null!;
        public Guid? TraceId { get; private set; }

        private Transaction() { }
        public Transaction(Guid accountId, decimal amount, TransactionTypeEnum transactionType, CurrencyEnum currency = CurrencyEnum.Pen, string description = "", Guid? traceId = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            if (accountId == Guid.Empty)
                throw new ArgumentException("Account id is required.", nameof(accountId));

            Id = Guid.NewGuid();
            AccountId = accountId;
            Amount = amount;
            Currency = currency;
            TransactionType = transactionType;
            DateUtc = DateTimeOffset.UtcNow;
            Description = description;
            TraceId = traceId;
        }

    }
}
