using API.Models;

namespace API.Services
{
    public class LongNumberService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public int[] GenerateSequence(int level)
        {
            var random = new Random();
            return Enumerable.Range(0, level).Select(_ => random.Next(0, 9)).ToArray();
        }

        public int CalculateScore(int[] guessed, int[] correct, int level = 1)
        {
            bool isCorrect = guessed.SequenceEqual(correct);
            return isCorrect ? level : 0;
        }

        public async Task SaveScoreAsync(UserScore userScore)
        {
            _context.UserScores.Add(userScore);
            await _context.SaveChangesAsync();
        }
    }
}
