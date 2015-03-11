using CertainDeathEngine.Models;

namespace CertainDeathEngine.DAL
{
    public interface IGameDAL
    {
        Game CreateGame();
        Game CreateGame(int worldId);
        bool SaveGame(Game game);
    }
}
