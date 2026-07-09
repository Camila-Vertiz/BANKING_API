using Banking.Domain.Enums;

namespace Banking.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public String UserName { get; private set; } = null!;
        public String PasswordHash { get; private set; } = null!;
        public UserRoleEnum Role { get; private set; }
        public bool isActive { get; private set; }
        public DateTimeOffset CreatedAtUtc { get; private set; }
        private User() { }
        public User(String userName, String passwordHash, UserRoleEnum role)
        {
            Id = Guid.NewGuid();
            UserName = userName;
            PasswordHash = passwordHash;
            Role = role;
            isActive = true;
            CreatedAtUtc = DateTimeOffset.UtcNow;
        }
        public void Deactivate()
        {
            isActive = false;
        }
        public void ChangePassword(String newPasswordHash)
        {
            PasswordHash = newPasswordHash;
        }
    }
}
