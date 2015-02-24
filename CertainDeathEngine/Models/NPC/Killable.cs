using CertainDeathEngine.Models.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.NPC
{
    [Serializable]
    public class Killable : GameObject
    {
        public int HealthPoints { get; set; }
        public int MaxHealthPoints { get; set; }
    }
}
