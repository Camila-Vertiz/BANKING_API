namespace Banking.Application.Request.Customer
{
    public class UpdateCustomerRequest
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
