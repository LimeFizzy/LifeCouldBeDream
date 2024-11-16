namespace API.Interfaces
{
    public interface ILongNumberService : IMasterGamesService
    {
        int[] GenerateSequence(int level);
        int CalculateScore(int level = 1);
    }
}
