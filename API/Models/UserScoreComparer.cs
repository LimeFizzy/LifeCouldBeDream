using API.Models;

public class UserScoreComparer : IComparable<UserScoreComparer>
{
    public UserScore UserScore { get; }

    public UserScoreComparer(UserScore userScore)
    {
        UserScore = userScore;
    }

    public int CompareTo(UserScoreComparer? other)          // 10. Use ICompare for sorting scores
    {
        if (other == null) return 1;

        // First, compare by Score (descending)
        int scoreComparison = other.UserScore.Score.CompareTo(UserScore.Score);
        if (scoreComparison != 0) return scoreComparison;

        // If scores are the same, compare by GameDate (ascending)
        return UserScore.GameDate.CompareTo(other.UserScore.GameDate);
    }
}
