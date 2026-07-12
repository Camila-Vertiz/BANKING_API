using Banking.Domain.Enums;

namespace Banking.Application.Responses
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }

        public Guid AccountId { get; set; }

        public TransactionTypeEnum TransactionType { get; set; }

        public decimal Amount { get; set; }

        public CurrencyEnum Currency { get; set; }

        public DateTimeOffset DateUtc { get; set; }

        public string Description { get; set; } = null!;

        public Guid? TraceId { get; set; }
    }
}