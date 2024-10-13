namespace API.Extensions
{
    public static class ArrayExtensions
    {
        public static bool IsValidSequence(this int[] sequence, int level)    // 5. Extension method usage
        {
            return sequence.Length == level && sequence.All(num => num >= 0 && num <= 9);
        }
    }
}
