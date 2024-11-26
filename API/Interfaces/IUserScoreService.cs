using API.Models;

namespace API.Interfaces
{
    public interface IUserScoreService
    {
        IEnumerable<UserScore> GetLeaderboard();
        Task SaveScoreAsync(UserScore userScore);
        GameTypes? GetGameTypeFromString(string gameType);
        Task<UserScore> GetScoreByIdAsync(int scoreId);
        Task DeleteScoreAsync(UserScore score);
    }
}
