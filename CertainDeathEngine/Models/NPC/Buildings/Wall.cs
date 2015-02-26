using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    public class Wall : Building
    {
        public Wall(Tile tile)
        {
            Type = BuildingType.WALL;
            Tile = tile;
        }

        public override void Update(long millis)
        {
            return;
            throw new NotImplementedException();
        }
    }
}
