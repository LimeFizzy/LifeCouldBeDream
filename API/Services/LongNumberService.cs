using System;
using System.Linq;
using API.Interfaces;
using API.Extensions;

namespace API.Services
{
    public class LongNumberService : ILongNumberService
    {
        private readonly ILogger<LongNumberService> _logger;

        public LongNumberService(ILogger<LongNumberService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private static readonly SequenseGenerator _seqGen = new();

        public int[] GenerateSequence(int level)
        {
            if (level <= 0)
            {
                _logger.LogWarning("GenerateSequence called with invalid level {Level}. Level must be greater than zero.", level);
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than zero.");
            }

            try
            {
                _logger.LogDebug("Generating sequence for level {Level}.", level);

                var random = new Random();
                var sequence = _seqGen.GenerateSequence(level, random => random.Next(0, 10));

                _logger.LogDebug("Successfully generated sequence for level {Level}: {Sequence}", level, string.Join(",", sequence));
                return sequence;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while generating sequence for level {Level}.", level);
                throw; // Re-throw the exception to propagate it up the stack
            }
        }

        public int CalculateScore(int level = 1)
        {
            if (level <= 0)
            {
                _logger.LogWarning("CalculateScore called with invalid level {Level}. Level must be greater than zero.", level);
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than zero.");
            }

            try
            {
                _logger.LogDebug("Calculating score for level {Level}.", level);

                int score = level - 1;

                _logger.LogDebug("Score for level {Level} is {Score}.", level, score);
                return score;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while calculating score for level {Level}.", level);
                throw; // Re-throw the exception to propagate it up the stack
            }
        }
    }
}
