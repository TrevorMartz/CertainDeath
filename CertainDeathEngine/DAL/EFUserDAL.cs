using System;
using CertainDeathEngine.DB;
using CertainDeathEngine.Models.User;
using log4net;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
            try
            {
                if (cdDBModel.Users.Count(x => x.FBUser.Id.Equals(fbUser.Id)) == 0)
                {
                    CertainDeathUser newUser = new CertainDeathUser() {FBUser = fbUser, WorldId = -1};
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
            catch (Exception e)
            {
                Log.Error("Failed creating a game user: " + e.Message);
            }
            return null;
        }

        public void UpdateGameUser(CertainDeathUser cdUser)
        {
            cdDBModel.Entry(cdUser).State = EntityState.Modified;
            cdDBModel.SaveChanges();
        }

        public IEnumerable<CertainDeathUser> GetAllUsers()
        {
            return cdDBModel.Users.Include("FBUser").Select(x => x).ToList();
        }

        public IEnumerable<MyAppUser> GetAllFbUsers()
        {
            return cdDBModel.FBUsers.Select(x => x);
        }

        public CertainDeathUser GetGameUser(int userid)
        {
            return (cdDBModel.Users.FirstOrDefault(x => x.Id == userid));
        }

        public CertainDeathUser GetGameUser(MyAppUser fbUser)
        {
            return (cdDBModel.Users.FirstOrDefault(x => x.FBUser.Id.Equals(fbUser.Id)));
        }

        public void GiveGameUserAGameWorldId(int userId, int worldId)
        {
            try
            {
                CertainDeathUser u = cdDBModel.Users.Include("FBUser").FirstOrDefault(x => x.Id == userId);
                u.WorldId = worldId;
                cdDBModel.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                Log.Error("DbSerialization error: " +
                          ex.EntityValidationErrors.FirstOrDefault().ValidationErrors.FirstOrDefault().ErrorMessage);
            }
            catch (Exception e)
            {
                Log.Error("Failed to give a user a world: " + e.Message);
            }
        }
    }
}
