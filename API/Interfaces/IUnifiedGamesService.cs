using API.Services;
using API.Models;

namespace API.Interfaces
{
    public interface IUnifiedGamesService<Type> where Type: struct
    {
        public Type[] GenerateSequence<T>(T service, int level) where T: IMasterGamesService;
        public int CalculateScore<T>(T service, int level) where T: IMasterGamesService;
    }
}