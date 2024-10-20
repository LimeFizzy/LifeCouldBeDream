using System;

namespace API.Services
{
    public class SequenceService
    {
        private static Random _random = new Random();

        public int[] GenerateSequence(int level)
        {
            return Enumerable.Range(0, level).Select(_ => _random.Next(1, 10)).ToArray();
        }

        // full score = level! (factorial)
        public int CalculateScore(int level)
        {
            return (level * (level + 1)) / 2;
        }
    }
}
