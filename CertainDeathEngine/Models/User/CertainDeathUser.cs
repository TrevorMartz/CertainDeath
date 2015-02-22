using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertainDeathEngine.Models.User
{
    [Serializable]
    public class CertainDeathUser
    {
        public MyAppUser FBUser { get; set; }
        public int WorldId { get; set; }
    }
}
