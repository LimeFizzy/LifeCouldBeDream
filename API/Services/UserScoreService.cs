using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserScoreService : IUserScoreService
    {
        private readonly AppDbContext _dbContext;

        public UserScoreService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IEnumerable<UserScore> GetLeaderboard()
        {
            // Get user scores from the database
            return _dbContext.UserScores.AsNoTracking();
        }
    }
}
