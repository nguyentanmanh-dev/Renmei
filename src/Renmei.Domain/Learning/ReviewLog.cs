namespace Renmei.Domain.Learning;

public class ReviewLog
{
    private ReviewLog() { }

    public ReviewLog(
        Guid userId,
        int kanjiId,
        Guid clientEventId,
        int rating,
        double prevEase,
        double newEase,
        int prevInterval,
        int newInterval,
        int? timeTakenMs,
        DateTime reviewedAtUtc)
    {
        if (!ReviewQuality.IsValid(rating))
        {
            throw new ArgumentOutOfRangeException(nameof(rating));
        }

        if (timeTakenMs is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(timeTakenMs));
        }

        UserId = userId;
        KanjiId = kanjiId;
        ClientEventId = clientEventId;
        Rating = rating;
        PrevEase = prevEase;
        NewEase = newEase;
        PrevInterval = prevInterval;
        NewInterval = newInterval;
        TimeTakenMs = timeTakenMs;
        ReviewedAt = reviewedAtUtc;
    }

    public long Id { get; private set; }
    public Guid UserId { get; private set; }
    public int KanjiId { get; private set; }
    public Guid ClientEventId { get; private set; }
    public int Rating { get; private set; }
    public double PrevEase { get; private set; }
    public double NewEase { get; private set; }
    public int PrevInterval { get; private set; }
    public int NewInterval { get; private set; }
    public int? TimeTakenMs { get; private set; }
    public DateTime ReviewedAt { get; private set; }
}