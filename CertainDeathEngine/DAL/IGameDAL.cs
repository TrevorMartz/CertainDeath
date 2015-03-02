using CertainDeathEngine.Models;

namespace CertainDeathEngine.DAL
{
    public interface IGameDAL
    {
        EngineInterface CreateGame();
        EngineInterface CreateGame(int worldId);
        bool SaveGame(Game game);
    }
}
