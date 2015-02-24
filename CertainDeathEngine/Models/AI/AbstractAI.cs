using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.AI
{
    public abstract class AbstractAI
    {
        public List<NPC.Killable> NPCs { get; set; }

        public AbstractAI()
        {
            this.NPCs = new List<NPC.Killable>();
        }

        public void AddNPC(NPC.Killable npc)
        {
            this.NPCs.Add(npc);
        }

        public void RemoveNPC(NPC.Killable npc)
        {
            this.NPCs.Remove(npc);
        }

        public abstract void Update();
    }
}
