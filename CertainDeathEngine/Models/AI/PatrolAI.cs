using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.AI
{
    public class PatrolAI : AbstractAI
    {
        public override void Update()
        {
            foreach (var n in NPCs)
            {
                // Move the NPC
                // n.Position = null
            }
        }
    }
}
