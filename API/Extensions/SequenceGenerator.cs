namespace API.Extensions
{
    public class SequenseGenerator
    {
        private readonly Random _random = new();

        public T[] GenerateSequence<T>(int level, Func<Random, T> generateItem) where T : struct
        {
            return Enumerable.Range(0, level).Select(_ => generateItem(_random)).ToArray();
        }
    }
}
