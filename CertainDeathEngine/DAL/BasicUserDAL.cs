using CertainDeathEngine.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.DAL
{
    public class BasicUserDAL : IUserDAL
    {
        public CertainDeathUser GetUser(MyAppUser fbUser)
        {
            return new CertainDeathUser() { WorldId = 7 };

        }
    }
}
