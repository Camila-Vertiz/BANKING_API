namespace Banking.Application.Requests.Transaction
{
    public class TransferRequest
    {
        public Guid FromAccountId { get; set; }

        public Guid ToAccountId { get; set; }

        public decimal Amount { get; set; }

        public string Description { get; set; } = null!;
    }
}