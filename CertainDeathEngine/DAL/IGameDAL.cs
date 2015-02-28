using CertainDeathEngine.Models;

namespace CertainDeathEngine.DAL
{
    public interface IGameDAL
    {
        bool SaveWorld(GameWorld world);

        EngineInterface LoadGame(int worldId);
    }
}
