namespace Banking.Application.Responses
{
    public class RegisterResponse
    {
        public Guid UserId { get; set; }
        public Guid CustomerId { get; set; }
        public string UserName { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTimeOffset CreatedAtUtc { get; set; }
    }
}