namespace API.Interfaces
{
    public interface IUnifiedGamesService<T>
    {
        T[] GenerateSequence(int level);
        int CalculateScore(int level);
    }
}
