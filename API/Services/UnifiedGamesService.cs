using API.Interfaces;
using API.Models;
using API.Extensions;
namespace API.Services
{
    public class UnifiedGamesService<Type>: IUnifiedGamesService<Type> where Type: struct
    {
        private readonly Random _random = new();

        public Type[] GenerateSequence<T>(T service, int level) where T: IMasterGamesService
        {
            if (service is ILongNumberService) {
                return Enumerable.Range(0, level).Select(_ => (Type)(object)_random.Next(0, 10)).ToArray();
            } else if (service is ISequenceService) {
                return Enumerable.Range(0, level).Select(_ => (Type)(object)new Square(_random.Next(1, 10), false)).ToArray();
            } else {
                throw new ArgumentException("Unsupported service type");
            }
        }
        public int CalculateScore<T>(T service, int level) where T: IMasterGamesService
        {
            if (service is ILongNumberService) {
                return level - 1;
            } else if (service is ISequenceService) {
                return level == 1 ? 0 : level * (level - 1) / 2;
            } else {
                throw new ArgumentException("Unsupported service type");
            }
        }
    }
}
