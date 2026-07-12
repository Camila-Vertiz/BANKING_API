using Banking.Domain.Enums;

namespace Banking.Application.Responses
{
    public class BalanceResponse
    {
        public Guid AccountId { get; set; }

        public decimal Balance { get; set; }

        public CurrencyEnum Currency { get; set; }
    }
}