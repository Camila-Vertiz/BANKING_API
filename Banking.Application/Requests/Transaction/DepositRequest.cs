namespace Banking.Application.Requests.Transaction
{
    public class DepositRequest
    {
        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; } = null!;
    }
}