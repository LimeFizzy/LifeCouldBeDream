using API.Interfaces;
using API.Models;
using System;
using System.Linq;

namespace API.Services
{
    public class SequenceService : ISequenceService
    {
        private static readonly Random _random = new Random();

        public Square[] GenerateSequence(int level)
        {

            // "defense in depth" - therefore level is being checked in service and in controller (not sure if we really need that)
            if (level <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than zero.");
            }

            try
            {
                return Enumerable.Range(0, level)
                    .Select(_ => new Square(_random.Next(1, 10), false))
                    .ToArray();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while generating the sequence.", ex);
            }
        }

        public int CalculateScore(int level)
        {
            // Ensure level is valid before calculating
            if (level < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than or equal to 1.");
            }

            return level == 1 ? 0 : level * (level - 1) / 2;
        }
    }
}
