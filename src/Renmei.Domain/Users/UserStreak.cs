namespace Renmei.Domain.Users;

public class UserStreak
{
    private UserStreak() { }

    public UserStreak(Guid userId)
    {
        UserId = userId;
    }

    public Guid UserId { get; private set; }
    public int CurrentStreak { get; private set; }
    public int LongestStreak { get; private set; }
    public DateOnly? LastStudyDate { get; private set; }
    public int StreakFreezeCount { get; private set; }
    public DateOnly? LastFreezeGrantMonth { get; private set; }
}