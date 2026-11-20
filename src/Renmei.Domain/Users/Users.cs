using Renmei.Domain.Enums;

namespace Renmei.Domain.Users;

public class User
{
    private readonly List<UserRole> _roles = [];

    private User() { }

    public User(
        string email,
        string passwordHash,
        string? displayName,
        string timeZone,
        DateTime nowUtc,
        int dailyNewCardGoal = 10)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash)) throw new ArgumentException("PasswordHash is required.", nameof(passwordHash));
        if (string.IsNullOrWhiteSpace(timeZone)) throw new ArgumentException("TimeZone is required.", nameof(timeZone));

        Id = Guid.NewGuid();
        Email = email.Trim();
        PasswordHash = passwordHash;
        DisplayName = displayName?.Trim();
        DailyNewCardGoal = dailyNewCardGoal;
        TimeZone = timeZone;
        Status = UserStatus.Active;
        EmailConfirmed = false;
        CreatedAt = nowUtc;
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string? DisplayName { get; private set; }
    public int? CurrentLevel { get; private set; }
    public int DailyNewCardGoal { get; private set; }
    public string TimeZone { get; private set; } = null!;
    public UserStatus Status { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyCollection<UserRole> Roles => _roles;

    public User(
        string email,
        string passwordHash,
        string? displayName,
        DateTime nowUtc,
        bool emailConfirmed,
        string timeZone = "UTC")
        : this(email, passwordHash, displayName, timeZone, nowUtc)
    {
        EmailConfirmed = emailConfirmed;
    }

    public void AddRole(int roleId) => _roles.Add(new UserRole(Id, roleId));
}