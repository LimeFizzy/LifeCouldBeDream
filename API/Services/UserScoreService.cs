using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserScoreService
    {
        private readonly AppDbContext _dbContext;

        public UserScoreService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Get the leaderboard (All scores)
        public IEnumerable<UserScore> GetLeaderboard()
        {
            // Get user scores from the database
            return _dbContext.UserScores.AsNoTracking();  
        }
    }
}
