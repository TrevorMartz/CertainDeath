﻿using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CertainDeathEngine.Factories
{
	public class MonsterGenerator : Temporal
	{
		private GameFactory Factory;
		private List<Tile> Tiles;
		private long Age = 0;
		private bool Delaying = true;

		// Delay in Millis before the monsters come
		public int Delay { get; set; }

		// Millis between each monster spawn
		public int Rate { get; set; }

		// How many monsters spawn after the delay is over
		public int InitialSpawnSize { get; set; }

		// How many monsters spawn after the initial spawn
		public int SpawnSize { get; set; }

		public MonsterGenerator(GameFactory factory, List<Tile> tiles)
		{
			Factory = factory;
			Tiles = tiles;
		}

		// This will need to return something or have a call back
		// To notify the game that there are new monsters
		// This will be used for sending back Delta JSON
		public void Update(long millis)
		{
			long nextTime = Age + millis;
			long timeLeft = millis;
			if (Delaying && nextTime > Delay)
			{
				Delaying = false;
				timeLeft = nextTime - Delay;
				for (int i = 0; i < InitialSpawnSize; i++ )
					BuildMonster(null);
			}
			if(!Delaying && timeLeft > Rate) {
				int monsters = (int)timeLeft / Rate;
				for (int i = 0; i < monsters; i++)
					for (int j = 0; i < SpawnSize; i++ )
						BuildMonster(null);
			}
			Age = nextTime;
		}
		public Monster BuildMonster(string monsterType)
		{
			// top (x, 0)
			// bottom (x, Tile.TOTAL_PIXELS)
			// left (0, y)
			// right (Tile.TOTAL_PIXELS, y)

			Tile randTile = Tiles[RandomGen.Random.Next(Tiles.Count())];
			int squareIndex = RandomGen.Random.Next(Tile.SQUARES);
			
			Point Position = new Point(
				squareIndex % Tile.SQUARE_SIZE * Square.PIXEL_SIZE,
				squareIndex / Tile.SQUARE_SIZE * Square.PIXEL_SIZE
			);

			Point Goal = new Point(Tile.TOTAL_PIXELS / 2, Tile.TOTAL_PIXELS / 2);
			int Speed = 25;
			Monster m = new Monster(randTile, Position, Goal, Speed) { Id = Factory.GetNextId()};
			randTile.AddObject(m);
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
