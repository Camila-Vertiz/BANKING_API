using Banking.Domain.Enums;

namespace Banking.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string UserName { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public UserRoleEnum Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTimeOffset CreatedAtUtc { get; private set; }
        private User() { }
        public User(string userName, string passwordHash, UserRoleEnum role)
        {
            Id = Guid.NewGuid();
            UserName = userName;
            PasswordHash = passwordHash;
            Role = role;
            IsActive = true;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }
        public void Deactivate()
        {
            IsActive = false;
        }
        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }
    }
}
