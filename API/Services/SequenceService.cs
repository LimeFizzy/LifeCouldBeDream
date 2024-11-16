using API.Interfaces;
using API.Models;
using API.Extensions;
namespace API.Services
{
    public class SequenceService : ISequenceService
    {
        private static readonly SequenseGenerator _seqGen = new();

        public Square[] GenerateSequence(int level)
        {
            return _seqGen.GenerateSequence(level, random => new Square(random.Next(1, 10), false));
        }

        public int CalculateScore(int level)
        {
            return level == 1 ? 0 : level * (level - 1) / 2;
        }
    }
}
