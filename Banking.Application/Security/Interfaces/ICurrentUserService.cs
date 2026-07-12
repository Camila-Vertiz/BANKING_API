namespace Banking.Application.Security.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string? UserName { get; }
        string? Role { get; }
    }
}
