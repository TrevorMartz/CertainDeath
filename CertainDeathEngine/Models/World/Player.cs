using CertainDeathEngine.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.User
{
    [Serializable]
    public class Player
    {
        public List<Resource> Resources { get; set; }

    }
}
