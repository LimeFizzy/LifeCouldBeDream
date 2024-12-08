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
            else if (typeof(T) == typeof(SquareChimp))
            {
                var allCoordinates = new List<(int X, int Y)>();
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        allCoordinates.Add((x, y));
                    }
                }

                var shuffled = allCoordinates.OrderBy(_ => _random.Next()).ToList();
                var chosenCoords = shuffled.Take(level).ToList();

                var squares = chosenCoords.Select((coord, index) =>
                    new SquareChimp(
                        number: index + 1,
                        revealed: false,
                        x: coord.X,
                        y: coord.Y
                    )
                ).ToArray();

                return (T[])(object)squares;
            }
            else
            {
                return [];
            }

        }

        public int CalculateScore(int level)
        {
            if (level < 1)
            {
                _logger.LogWarning("CalculateScore called with invalid level {Level}. Level must be greater than or equal to 1.", level);
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than or equal to 1.");
            }


            if (typeof(T) == typeof(int))
            {
                return level - 1;
            }
            else if (typeof(T) == typeof(Square))
            {
                level--;
                return level <= 2 ? level : level * (level - 1) / 2;
            }
            else if (typeof(T) == typeof(SquareChimp))
            {
                level--;
                return (level * level  +  5 * level) / 2;
            }
            else
            {
                return level;
            }
        }
    }
}
