using Banking.Domain.Enums;

namespace Banking.Application.Requests.BankAccount
{
    public class CreateBankAccountRequest
    {
        public Guid CustomerId { get; set; }

        public CurrencyEnum Currency { get; set; }
    }
}