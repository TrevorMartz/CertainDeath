using System.Collections.Generic;
using CertainDeathEngine.Models.User;

namespace CertainDeathEngine.DAL
{
    public interface IUserDAL
    {
        CertainDeathUser CreateGameUser(MyAppUser fbUser);

        void UpdateGameUser(CertainDeathUser cdUser);

        IEnumerable<CertainDeathUser> GetAllUsers();

        IEnumerable<MyAppUser> GetAllFbUsers();

        CertainDeathUser GetGameUser(int userid);

        CertainDeathUser GetGameUser(MyAppUser fbUser);

        void GiveGameUserAGameWorldId(int userId, int worldId);
    }
}
