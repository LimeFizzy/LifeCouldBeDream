using API.Interfaces;
using API.Models;
namespace API.Services
{
    public class SequenceService : ISequenceService
    {
        private static Random _random = new Random();

        public Square[] GenerateSequence(int level)
        {
            return Enumerable.Range(0, level).Select(_ => new Square(_random.Next(1, 10), false)).ToArray();
        }

        public int CalculateScore(int level)
        {
            return level == 1 ? 0 : level * (level - 1) / 2;
        }
    }
}
