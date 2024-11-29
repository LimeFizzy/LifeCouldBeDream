using API.Models;
using API.Interfaces;

namespace API.Services
{
    public class UnifiedGamesService<T>(ILogger<UnifiedGamesService<T>> logger) : IUnifiedGamesService<T>
    {
        private readonly Random _random = new();
        private readonly ILogger<UnifiedGamesService<T>> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public T[] GenerateSequence(int level)
        {
            if (level <= 0)
            {
                _logger.LogWarning("GenerateSequence called with invalid level {Level}. Level must be greater than zero.", level);
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than zero.");
            }

            try
            {
                if (typeof(T) == typeof(int))
                {
                    return Enumerable.Range(0, level)
                        .Select(_ => (T)(object)_random.Next(0, 10))
                        .ToArray();
                }
                else if (typeof(T) == typeof(Square))
                {
                    return Enumerable.Range(0, level)
                        .Select(_ => (T)(object)new Square(_random.Next(1, 10), false))
                        .ToArray();
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported type {typeof(T)} for sequence generation.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the sequence for level {Level}.", level);
                throw new InvalidOperationException("An error occurred while generating the sequence.", ex);
            }
        }

        public int CalculateScore(int level)
        {
            if (level < 1)
            {
                _logger.LogWarning("CalculateScore called with invalid level {Level}. Level must be greater than or equal to 1.", level);
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than or equal to 1.");
            }

            try
            {
                if (typeof(T) == typeof(int))
                {
                    return level - 1;
                }
                else if (typeof(T) == typeof(Square))
                {
                    level--;
                    return level <= 2 ? level : level * (level - 1) / 2;
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported type {typeof(T)} for score calculation.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while calculating the score for level {Level}.", level);
                throw new InvalidOperationException("An error occurred while calculating the score.", ex);
            }
        }
    }
}
