using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
	class FireOfLife : Building
	{
		public FireOfLife(Tile tile) : base(tile, new Point(Tile.TOTAL_PIXELS / 2, Tile.TOTAL_PIXELS / 2))
		{
            Type = BuildingType.FIREOFLIFE;
			// I think its okay if this building doesn't have to snap to the
			// square grid. It's special and should be in the very center.
			// (If it isn't in the center, and it's small, the monsters might                  Too bad.
			//    walk past it without hitting it, they are aiming for the exact
			//    center of the tile and a 20 by 20 doesn't have an exact center)
            MaxLevel = 5;
            Level = 0;
            Upgrade();
		}

        public override void Upgrade()
        {
            if (Level < MaxLevel)
            {
                Level++;
                Width = Square.PIXEL_SIZE * Level;
                Height = Square.PIXEL_SIZE * Level;
                MaxHealthPoints = 100 * Level;
                HealthPoints = MaxHealthPoints;
            }
        }

        public override void Update(long millis)
        {
            return;
            throw new NotImplementedException();
        }
    }
}
