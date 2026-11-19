namespace Renmei.Domain.Users;

public class Role
{
    private Role() { }

    public Role(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Role name is required.", nameof(name));
        Name = name.Trim();
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = null!;
}