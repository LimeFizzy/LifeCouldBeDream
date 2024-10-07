using API.Models;

namespace API.Services
{
    public interface IUserScoreService
    {
        IEnumerable<UserScore> GetLeaderboard();
    }
}