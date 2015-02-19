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

        public GameWorld()
        {
            this.CurrentTile = new Tile();
        }

        public GameWorld(Tile t)
        {
            this.CurrentTile = t;
        }

        public void AddObject(GameObject obj)
        {
            this.CurrentTile.AddObject(obj);
        }
    }
}
