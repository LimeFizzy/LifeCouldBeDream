using API.Interfaces;
using System;

namespace API.Services
{
    public class LongNumberService : ILongNumberService
    {
        public int[] GenerateSequence(int level)
        {
            if (level <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than zero.");
            }

            var random = new Random();
            return Enumerable.Range(0, level).Select(_ => random.Next(0, 10)).ToArray(); // 9. LINQ usage
        }

        public int CalculateScore(int level = 1) // 4. Optional argument usage
        {
            if (level <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than zero.");
            }
            return level - 1;
        }
    }
}
