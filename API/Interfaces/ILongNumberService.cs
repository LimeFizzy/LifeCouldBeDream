namespace API.Interfaces
{
    public interface ILongNumberService
    {
        int[] GenerateSequence(int level);
        int CalculateScore(int level = 1);
    }
}
