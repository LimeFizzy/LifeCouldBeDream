using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Data;

namespace API.Services
{
    public class UserScoreService(AppDbContext dbContext) : IUserScoreService
    {
        private readonly AppDbContext _dbContext = dbContext;

        public IEnumerable<UserScore> GetLeaderboard()
        {
            return _dbContext.UserScores.AsNoTracking();
        }
    }
}
