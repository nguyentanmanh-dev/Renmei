using Renmei.Domain.Learning;

namespace Renmei.Domain.Tests.Learning;

public class LearningDomainTests
{
    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, true)]
    [InlineData(5, true)]
    [InlineData(6, false)]
    public void ReviewQuality_IsValid_enforces_zero_to_five_range(int rating, bool expected)
    {
        Assert.Equal(expected, ReviewQuality.IsValid(rating));
    }

    [Theory]
    [InlineData(2, false)]
    [InlineData(3, true)]
    [InlineData(4, true)]
    public void ReviewQuality_IsRecall_checks_threshold_three(int rating, bool expected)
    {
        Assert.Equal(expected, ReviewQuality.IsRecall(rating));
    }

    [Fact]
    public void ReviewQuality_button_constants_are_valid_and_forgot_is_not_recall()
    {
        Assert.True(ReviewQuality.IsValid(ReviewQuality.ButtonForgot));
        Assert.True(ReviewQuality.IsValid(ReviewQuality.ButtonHard));
        Assert.True(ReviewQuality.IsValid(ReviewQuality.ButtonGood));
        Assert.True(ReviewQuality.IsValid(ReviewQuality.ButtonEasy));
        Assert.False(ReviewQuality.IsRecall(ReviewQuality.ButtonForgot));
    }

    [Fact]
    public void ReviewLog_throws_when_rating_is_out_of_range()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ReviewLog(Guid.NewGuid(), 1, Guid.NewGuid(), 6, 2.5, 2.5, 0, 1, 500, DateTime.UtcNow));
    }

    [Fact]
    public void ReviewLog_throws_when_timeTakenMs_is_negative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new ReviewLog(Guid.NewGuid(), 1, Guid.NewGuid(), 4, 2.5, 2.5, 0, 1, -1, DateTime.UtcNow));
    }

    [Fact]
    public void ReviewLog_allows_null_timeTakenMs()
    {
        var log = new ReviewLog(Guid.NewGuid(), 1, Guid.NewGuid(), 4, 2.5, 2.5, 0, 1, null, DateTime.UtcNow);
        Assert.Null(log.TimeTakenMs);
    }

    [Fact]
    public void KanjiProgressState_contains_Mastered()
    {
        Assert.True(Enum.IsDefined(KanjiProgressState.Mastered));
    }

    [Fact]
    public void UserKanjiProgress_initializes_with_default_ease_and_new_state()
    {
        var progress = new UserKanjiProgress(Guid.NewGuid(), 1);
        Assert.Equal(2.5, progress.EaseFactor);
        Assert.Equal(KanjiProgressState.New, progress.State);
    }
}