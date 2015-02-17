using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.AI
{
    public abstract class AbstractAI
    {
        public List<NPC.NPC> NPCs { get; set; }

        public AbstractAI()
        {
            this.NPCs = new List<NPC.NPC>();
        }

        public void AddNPC(NPC.NPC npc)
        {
            this.NPCs.Add(npc);
        }

        public void RemoveNPC(NPC.NPC npc)
        {
            this.NPCs.Remove(npc);
        }

        public abstract void Update();
    }
}
