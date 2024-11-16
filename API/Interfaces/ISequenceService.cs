using API.Models;

namespace API.Interfaces
{
    public interface ISequenceService : IMasterGamesService
    {
        Square[] GenerateSequence(int level);
        int CalculateScore(int level);
    }
}
