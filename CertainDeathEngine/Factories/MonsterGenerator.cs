﻿using CertainDeathEngine.Models;
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
			InitialSpawnSize = 5;
			SpawnSize = 1; 
			Delay = 20000;
			Rate = 10000;
		}

        long elapsed = 0;
		public void Update(long millis)
		{
            elapsed += millis;
			_time += millis;
			if (_delaying && _time > Delay)
			{
				_delaying = false;
				SpawnMonsters(InitialSpawnSize);
				_time -= Delay;
			}
			if(!_delaying && _time > Rate) {
				int monsters = (int)(_time / Rate);// * elapsed / 1000);
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
            Monster m;
			Tile randTile;
            lock (World)
            {
                randTile = World.Tiles[RandomGen.Random.Next(World.Tiles.Count())];
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
                    Damage = 10
                };
                randTile.AddObject(m);
            }
			if(randTile == World.CurrentTile)
				this.World.AddUpdateMessage(new PlaceMonsterUpdateMessage(m.Id)
				{
					PosX = m.Position.X,
					PosY = m.Position.Y,
					Type = m.Name,
					Direction = m.Direction,
					State = m.Status
				});
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
