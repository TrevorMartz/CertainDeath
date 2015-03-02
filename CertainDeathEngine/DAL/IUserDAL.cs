using CertainDeathEngine.Models.User;

namespace CertainDeathEngine.DAL
{
    public interface IUserDAL
    {
        CertainDeathUser CreateGameUser(MyAppUser fbUser);

        CertainDeathUser GetGameUser(MyAppUser fbUser);

        void GiveGameUserAGameWorldId(int userId, int worldId);
    }
}
