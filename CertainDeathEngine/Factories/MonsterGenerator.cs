using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC;
using log4net;
using System;
using System.Linq;
using System.Windows;

namespace CertainDeathEngine.Factories
{
	public class MonsterGenerator : GameFactory, Temporal
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		
		private long _time = 0;
		private bool _delaying = true;

		// Delay in Millis before the monsters come
		public int Delay { get; set; }

		// Millis between each monster spawn
		public int Rate { get; set; }

		// How many monsters spawn after the delay is over
		public int InitialSpawnSize { get; set; }

		// How many monsters spawn after the initial spawn
		public int SpawnSize { get; set; }

		public MonsterGenerator(GameWorld world) : base(world)
		{
			World = world;
		}

		// This will need to return something or have a call back
		// To notify the game that there are new monsters
		// This will be used for sending back Delta JSON
		public void Update(long millis)
		{
			_time += millis;
			if (_delaying && _time > Delay)
			{
				_delaying = false;
				SpawnMonsters(InitialSpawnSize);
				_time -= Delay;
			}
			if(!_delaying && _time > Rate) {
				int monsters = (int)_time / Rate;
				for (int i = 0; i < monsters; i++)
					SpawnMonsters(SpawnSize);
				_time -= monsters * Rate;
			}
		}


		public void SpawnMonsters(int num)
		{
			for (int i = 0; i < num; i++)
				BuildMonster(null);
		}

		public Monster BuildMonster(string monsterType)
		{
			// top (x, 0)
			// bottom (x, Tile.TOTAL_PIXELS)
			// left (0, y)
			// right (Tile.TOTAL_PIXELS, y)
            Monster m;
            lock (World)
            {
                Tile randTile = World.Tiles[RandomGen.Random.Next(World.Tiles.Count())];
                int squareIndex = RandomGen.Random.Next(Tile.SQUARES);

                Point Position = new Point(
                    squareIndex % Tile.SQUARES_PER_ROW_AND_COLUMN * Square.PIXEL_SIZE,
                    squareIndex / Tile.SQUARES_PER_ROW_AND_COLUMN * Square.PIXEL_SIZE
                );

				Point Goal = new Point(Tile.TOTAL_PIXELS / 2 + Square.PIXEL_SIZE / 2, Tile.TOTAL_PIXELS / 2 + Square.PIXEL_SIZE / 2);
                int Speed = 25;
                m = new Monster(randTile, Position, Goal, Speed, (MonsterName)RandomGen.Random.Next(Enum.GetValues(typeof(MonsterName)).Length))
                {
                    Id = GetNextId(),
                    Damage = 1
                };
                randTile.AddObject(m);
            }
			return m;
		}

		//Random edge of a tile
		//int side = RandomGen.Random.Next() % 4;
		//	int placement = RandomGen.Random.Next(Tile.TOTAL_PIXELS - 2);
		//	Point Position;
		//	if (side % 2 == 0)
		//	{
		//		Position = new Point(placement,
		//			side == 1 ? 1 : Tile.TOTAL_PIXELS - 2);
		//	}
		//	else
		//	{
		//		Position = new Point(
		//			side == 3 ? 1 : Tile.TOTAL_PIXELS - 2,
		//			placement);
		//	}

	}
}
