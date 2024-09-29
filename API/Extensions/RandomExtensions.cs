namespace API.Extensions
{
    public static class RandomExtensions
    {
        // Extension method to generate a random sequence of integers
        public static int[] GenerateRandomSequence(this Random random, int level)
        {
            return Enumerable.Range(0, level).Select(_ => random.Next(0, 10)).ToArray();
        }
    }
}
