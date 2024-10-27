using System;

namespace API.Services
{
    public struct Square
    {
        public int Id { get; }
        public bool IsActive { get; }

        public Square(int id, bool isActive)
        {
            Id = id;
            IsActive = isActive;
        }
    }
    public class SequenceService
    {
        private static Random _random = new Random();

        public Square[] GenerateSequence(int level)
        {
            return Enumerable.Range(0, level).Select(_ => new Square(_random.Next(1, 10), false)).ToArray();
        }

        public int CalculateScore(int level)
        {
            return (level * (level + 1)) / 2;
        }
    }
}
