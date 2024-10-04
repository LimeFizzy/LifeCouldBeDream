//using API.Data;
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
        public async Task<IEnumerable<UserScore>> GetLeaderboard()
        {
            return await _dbContext.UserScores.AsNoTracking().ToListAsync();
        }
    }
}
