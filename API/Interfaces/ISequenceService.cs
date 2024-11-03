using API.Models;

namespace API.Interfaces
{
    public interface ISequenceService
    {
        Square[] GenerateSequence(int level);
        int CalculateScore(int level);
    }
}
