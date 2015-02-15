using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models
{
    public class GameWorld
    {
        public Tile CurrentTile { get; set; }
        public List<GameObject> Objects { get; set; }
    }
}
