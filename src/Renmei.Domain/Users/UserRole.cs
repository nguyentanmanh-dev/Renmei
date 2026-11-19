namespace Renmei.Domain.Users;

public class UserRole
{
    private UserRole() { }

    public UserRole(Guid userId, int roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public Guid UserId { get; private set; }
    public int RoleId { get; private set; }
}