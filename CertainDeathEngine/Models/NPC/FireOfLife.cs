using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CertainDeathEngine.Models.NPC
{
	class FireOfLife : Building
	{
		public FireOfLife()
		{
			// I think its okay if this building doesn't have to snap to the
			// square grid. It's special and should be in the very center.
			// (If it isn't in the center, and it's small, the monsters might
			//    walk past it without hitting it, they are aiming for the exact
			//    center of the tile and a 20 by 20 doesn't have an exact center)
			Position = new Point(Tile.TOTAL_PIXELS / 2, Tile.TOTAL_PIXELS / 2);
			Level1();
		}

		public void Level1()
		{
			Width = Square.PIXEL_SIZE;
			Height = Square.PIXEL_SIZE;
			MaxHealthPoints = 100;
			HealthPoints = 100;
		}

		public void Level2()
		{
			Width = Square.PIXEL_SIZE * 2;
			Height = Square.PIXEL_SIZE * 2;
			MaxHealthPoints = 1000;
			HealthPoints = 1000;
		}

		public void Level3()
		{
			Width = Square.PIXEL_SIZE * 3;
			Height = Square.PIXEL_SIZE * 3;
			MaxHealthPoints = 10000;
			HealthPoints = 10000;
		}

		public void Level4()
		{
			Width = Square.PIXEL_SIZE * 4;
			Height = Square.PIXEL_SIZE * 4;
			MaxHealthPoints = 100000;
			HealthPoints = 100000;
		}

		public void Level5()
		{
			Width = Square.PIXEL_SIZE * 5;
			Height = Square.PIXEL_SIZE * 5;
			MaxHealthPoints = 1000000;
			HealthPoints = 1000000;
		}
	}
}
