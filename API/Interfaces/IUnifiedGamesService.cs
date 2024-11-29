using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{
    public interface IUnifiedGamesService<Type> where Type : struct
    {
        public Type[] GenerateSequence<T>(T controller, int level) where T : ControllerBase;
        public int CalculateScore<T>(T gameType, int level) where T : struct, System.Enum;
    }
}