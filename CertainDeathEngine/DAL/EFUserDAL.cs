using CertainDeathEngine.DB;
using CertainDeathEngine.Models.User;
using log4net;
using System.Collections.Generic;
using System.Linq;

namespace CertainDeathEngine.DAL
{
    public class EFUserDAL : IUserDAL
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CDDBModel cdDBModel;

        public EFUserDAL()
        {
            cdDBModel = new CDDBModel();
        }

        public CertainDeathUser CreateGameUser(MyAppUser fbUser)
        {
            if (cdDBModel.Users.Count(x => x.FBUser.Email.Equals(fbUser.Email)) == 0)
            {
                CertainDeathUser newUser = new CertainDeathUser() { FBUser = fbUser, WorldId = -1 };
                cdDBModel.Users.Add(newUser);
                cdDBModel.SaveChanges();
                return newUser;
            }
            else
            {
                // the user already existed, so return the existing user
                return GetGameUser(fbUser);
                // TODO - Unless we want to trow exceptions for someone else to deal with - blake
            }
        }

        public IEnumerable<CertainDeathUser> GetAllUsers()
        {
            return cdDBModel.Users.Include("FBUser").Include("HighScore").Select(x => x).ToList();
        }

        public IEnumerable<MyAppUser> GetAllFbUsers()
        {
            return cdDBModel.FBUsers.Select(x => x);
        }

        public CertainDeathUser GetGameUser(MyAppUser fbUser)
        {
            return (cdDBModel.Users.FirstOrDefault(x => x.FBUser.Id.Equals(fbUser.Id)));
        }

        public void GiveGameUserAGameWorldId(int userId, int worldId)
        {
            CertainDeathUser u = cdDBModel.Users.FirstOrDefault(x => x.Id == userId);
            u.WorldId = worldId;
            cdDBModel.SaveChanges();
        }
    }
}
