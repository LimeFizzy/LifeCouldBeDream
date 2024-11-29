using API.Models;
using API.Interfaces;
using API.Controllers;
using Microsoft.AspNetCore.Mvc;
namespace API.Services
{
    public class UnifiedGamesService<Type> : IUnifiedGamesService<Type> where Type : struct
    {
        private readonly Random _random = new();
        private readonly ILogger<UnifiedGamesService<Type>> _logger;
        public UnifiedGamesService(ILogger<UnifiedGamesService<Type>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Type[] GenerateSequence<T>(T controller, int level) where T : ControllerBase
        {
            if (level <= 0)
            {
                _logger.LogWarning("GenerateSequence called with invalid level {Level}. Level must be greater than zero.", level);
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than zero.");
            }

            try
            {
                if (controller is LongNumberController)
                {
                    return Enumerable.Range(0, level).Select(_ => (Type)(object)_random.Next(0, 10)).ToArray();
                }
                else if (controller is SequenceController)
                {
                    return Enumerable.Range(0, level).Select(_ => (Type)(object)new Square(_random.Next(1, 10), false)).ToArray();
                }
                else
                {
                    throw new ArgumentException("Unsupported game type");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the sequence for level {Level}.", level);
                throw new InvalidOperationException("An error occurred while generating the sequence.", ex);
            }
        }
        public int CalculateScore<T>(T gameType, int level) where T : struct, System.Enum
        {
            if (level < 1)
            {
                _logger.LogWarning("CalculateScore called with invalid level {Level}. Level must be greater than or equal to 1.", level);
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than or equal to 1.");
            }

            try
            {
                if (gameType is GameTypes.LONG_NUMBER)
                {
                    return level - 1;
                }
                else if (gameType is GameTypes.SEQUENCE)
                {
                    level--;
                    return level <= 2 ? level : level * (level - 1) / 2;
                }
                else
                {
                    throw new ArgumentException("Unsupported service type");
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
