using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Factories
{
    public class GameFactory
    {
        static int nextObjectId = 1;

        public GameFactory()
        {

        }

        public Building BuildBuilding(string buildingType)
        {
            return new Building() { Id = nextObjectId++ };
        }

        public Monster BuildMonster(string monsterType)
		{
			// top (x, 0)
			// bottom (x, Tile.TOTAL_PIXELS)
			// left (0, y)
			// right (Tile.TOTAL_PIXELS, y)
			int side = RandomGen.Random.Next() % 4;
			int placement = RandomGen.Random.Next(Tile.TOTAL_PIXELS - 1);
			Point Position;
			if(side % 2 == 0) 
			{
				Position = new Point(placement,
					side == 1 ? 0 : Tile.TOTAL_PIXELS - 1);
			}
			else
			{
				Position = new Point(
					side == 3 ? 0 : Tile.TOTAL_PIXELS,
					placement);
			}

			Point Goal = new Point(Tile.TOTAL_PIXELS / 2, Tile.TOTAL_PIXELS / 2);
			int Speed = 25;
			Monster m = new Monster(Position, Goal, Speed) { Id = nextObjectId++ };
			
			return m;
        }
    }
}
