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
            return Enumerable.Range(0, level).Select(_ => random.Next(0, 10)).ToArray();    // 9. LINQ to Objects usage
        }

        public int CalculateScore(int level = 1)    // 4. Optional argument usage
        {
            return level - 1;
        }

    }
}
