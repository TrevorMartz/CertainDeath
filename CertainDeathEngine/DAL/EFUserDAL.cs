using CertainDeathEngine.DB;
using CertainDeathEngine.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.DAL
{
    public class EFUserDAL : IUserDAL
    {
        private CDDBModel cdDBModel;

        public EFUserDAL()
        {
            cdDBModel = new CDDBModel();
        }

        public CertainDeathUser CreateGameUser(MyAppUser fbUser)
        {
            if (cdDBModel.Users.Where(x => x.FBUser.Email.Equals(fbUser.Email)).Count() == 0)
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

        public CertainDeathUser GetGameUser(MyAppUser fbUser)
        {
            return (cdDBModel.Users.Where(x => x.FBUser.Id.Equals(fbUser.Id)).FirstOrDefault());
        }

    }
}
