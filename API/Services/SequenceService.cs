namespace API.Services
{
    public struct Square(int id, bool isActive)
    {
        public int Id { get; } = id;
        public bool IsActive { get; } = isActive;
    }
    public class SequenceService
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
