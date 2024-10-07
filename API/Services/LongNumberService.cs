using API.Models;
using API.Data;

namespace API.Services
{
    public class LongNumberService
    {
        private readonly AppDbContext _context;

        public LongNumberService(AppDbContext context)
        {
            _context = context;
        }


        public int[] GenerateSequence(int level)
        {
            var random = new Random();
            return Enumerable.Range(0, level).Select(_ => random.Next(0, 10)).ToArray();
        }

        public int CalculateScore(int[] guessed, int[] correct, int level = 1)
        {
            return level - 1;
        }

        public async Task SaveScoreAsync(UserScore userScore)
        {
            _context.UserScores.Add(userScore);
            await _context.SaveChangesAsync();
        }
    }
}
