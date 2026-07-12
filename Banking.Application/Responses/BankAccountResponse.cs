using Banking.Domain.Enums;

namespace Banking.Application.Responses
{
    public class BankAccountResponse
    {
        public Guid Id { get; set; }

        public string Number { get; set; } = null!;

        public decimal Balance { get; set; }

        public CurrencyEnum Currency { get; set; }

        public Guid CustomerId { get; set; }

        public BankAccountStatusEnum Status { get; set; }

        public DateTimeOffset CreatedAtUtc { get; set; }
    }
}