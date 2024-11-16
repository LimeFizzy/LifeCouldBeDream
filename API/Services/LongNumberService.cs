using API.Interfaces;
using API.Extensions;

namespace API.Services
{
    public class LongNumberService() : ILongNumberService
    {
        private static readonly SequenseGenerator _seqGen = new();

        public int[] GenerateSequence(int level)
        {
            return _seqGen.GenerateSequence(level, random => random.Next(0, 10));    // 9. LINQ to Objects usage
        }

        public int CalculateScore(int level = 1)    // 4. Optional argument usage
        {
            return level - 1;
        }

    }
}
