namespace Renmei.Domain.Learning;

public class UserKanjiProgress
{
    private UserKanjiProgress() { }

    public UserKanjiProgress(Guid userId, int kanjiId)
    {
        UserId = userId;
        KanjiId = kanjiId;
        EaseFactor = 2.5;
        IntervalDays = 0;
        Repetitions = 0;
        LapseCount = 0;
        State = KanjiProgressState.New;
        IsLeech = false;
    }

    public Guid UserId { get; private set; }
    public int KanjiId { get; private set; }
    public double EaseFactor { get; private set; }
    public int IntervalDays { get; private set; }
    public int Repetitions { get; private set; }
    public int LapseCount { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public KanjiProgressState State { get; private set; }
    public bool IsLeech { get; private set; }
}