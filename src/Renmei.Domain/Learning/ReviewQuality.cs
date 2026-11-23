namespace Renmei.Domain.Learning;

public static class ReviewQuality
{
    public const int Min = 0;
    public const int Max = 5;
    public const int RecallThreshold = 3;

    public const int ButtonForgot = 1;
    public const int ButtonHard = 3;
    public const int ButtonGood = 4;
    public const int ButtonEasy = 5;

    public static bool IsValid(int quality) => quality is >= Min and <= Max;
    public static bool IsRecall(int quality) => quality >= RecallThreshold;
}