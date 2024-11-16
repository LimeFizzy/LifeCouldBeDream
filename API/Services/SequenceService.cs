using API.Interfaces;
using API.Models;
using Microsoft.Extensions.Logging; // Required for ILogger
using System;
using System.Linq;

namespace API.Services
{
    public class SequenceService : ISequenceService
    {
        private static readonly Random _random = new Random();
        private readonly ILogger<SequenceService> _logger;

        public SequenceService(ILogger<SequenceService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Square[] GenerateSequence(int level)
        {
            // "Defense in depth" - level checked in both service and controller
            if (level <= 0)
            {
                _logger.LogWarning("GenerateSequence called with invalid level {Level}. Level must be greater than zero.", level);
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than zero.");
            }

            try
            {
                var sequence = Enumerable.Range(0, level)
                    .Select(_ => new Square(_random.Next(1, 10), false))
                    .ToArray();

                return sequence;
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
                int score = level == 1 ? 0 : level * (level - 1) / 2;

                return score;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while calculating the score for level {Level}.", level);
                throw new InvalidOperationException("An error occurred while calculating the score.", ex);
            }
        }
    }
}
