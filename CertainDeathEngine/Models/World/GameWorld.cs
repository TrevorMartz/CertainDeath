using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models
{
    public class GameWorld
    {
        public int Id { get; set; }

        public Tile CurrentTile { get; set; }

        public GameWorld(int worldId) : this(new Tile(), worldId) { }

        public GameWorld(Tile t, int worldId)
        {
            this.CurrentTile = t;
        }

        public void AddObject(GameObject obj)
        {
            this.CurrentTile.AddObject(obj);
        }
    }
}
