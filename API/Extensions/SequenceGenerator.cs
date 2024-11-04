namespace API.Extensions
{
    public class SequenseGenerator()
    {
        private readonly Random _random = new();

        public T[] GenerateTSequence<T>(int level, Func<Random, T> generateItem)
        {
            return Enumerable.Range(0, level).Select(_ => generateItem(_random)).ToArray();
        }
    }
}
