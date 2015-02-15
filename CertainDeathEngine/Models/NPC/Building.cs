using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.NPC
{
    public class Building : NPC
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public float HarvestRate { get; set; }
    }
}
