using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CertainDeathEngine.Models.NPC
{
    [Serializable]
	[JsonObject(MemberSerialization.OptIn)]
    public class Monster : Killable, Temporal
    {
		[JsonProperty]
		public string Type { get { return "Monster"; } }

		// Get the pixel location of the monster's goal
		// This is always on tile 0, 0. We shouldn't let them move the fire.
		public Point Goal { get; set; }

		// The Radius of the monster's colision circle
		public int Radius { get; set; }

		// Monster's movement speed in Pixels per second
		public float Speed { get; set; }

		// Direction to get to goal (As a normalized vector)
		public Point Direction { get; set; }

		// Monster needs to know where they are so they
		// can go towards the FireOfLife
		public Tile Tile { get; private set; }

		public Monster(Tile t, Point starting, Point goal, int speed)
		{
			Tile = t;
			Position = starting;
			Goal = goal;
			Speed = speed;
			double xDist = goal.X - Position.X;
			double yDist = goal.Y - Position.Y;
			double distance = Math.Sqrt(xDist * xDist + yDist * yDist);
			CalculateDirection();
		}
		public void Update(long millis)
		{
			Move(millis);
		}

		// Moves the monster the distance they would go after millisecond have elapsed
		// This will need to return something or call a callback to notify the front end
		// if a monster moves from one tile to another. (if we aren't sending the whole game)
		public void Move(long milliseconds)
		{
			// calculate how far the monster will move
			Point distance = new Point(
				Direction.X * Speed * (milliseconds / 1000.0),
				Direction.Y * Speed * (milliseconds / 1000.0));
			// add their movement to their current position
			Position = new Point(
				Position.X + distance.X,
				Position.Y + distance.Y);

			// If they have moved to another tile,
			if (Position.X < 0 || Position.X >= Tile.TOTAL_PIXELS ||
				Position.Y < 0 || Position.Y >= Tile.TOTAL_PIXELS)
			{
				
				if (Position.X < 0)
					MoveToTile(Tile.Left, new Point(Tile.TOTAL_PIXELS, 0));
				else if (Position.X >= Tile.TOTAL_PIXELS)
					MoveToTile(Tile.Right, new Point(-Tile.TOTAL_PIXELS, 0));

				if (Position.Y < 0)
					MoveToTile(Tile.Above, new Point(0, Tile.TOTAL_PIXELS));
				else if (Position.Y >= Tile.TOTAL_PIXELS)
					MoveToTile(Tile.Below, new Point(0, -Tile.TOTAL_PIXELS));
			}
		}

		private void MoveToTile(Tile tile, Point positionChange)
		{
			if (tile == null)
			{
				// Then the monster has walked off the universe.
				// Either it has killed the fire of life, the game
				// didn't end, and he kept walking.
				// or he is trying to get to the fire of life and walked
				// through a null tile. 
				Tile.Objects.Remove(this);
			}
			else {
				Tile.Objects.Remove(this);
				Tile = tile;
				tile.AddObject(this);
				Position = new Point(
					Position.X + positionChange.X,
					Position.Y + positionChange.Y);
			}
		}

		/* 
		 Monsters will always move directly to the FireOfLife on tile 0,0
		 This will cause problems if there is an empty tile between the monsters 
		 and the Fire of life:
		 * 
		 * T is  a tile, M is a tile with a monster, . is a null, F is the fire
		 * 
			TTTT...
		    TTFT...
		 *  TTTT...
		 *  TTTTTTM
		 *  
		 * This can be fixed by making the monsters smarter or limiting how the players discover tiles
		 * Or maybe we always have the world as a square, and let monsters walk through tiles 
		 * that the player hasen't discovered yet.
		 */
		private void CalculateDirection()
		{
			double xDist = Goal.X - (Position.X + Tile.Position.X * Tile.TOTAL_PIXELS);
			double yDist = Goal.Y - (Position.Y + Tile.Position.Y * Tile.TOTAL_PIXELS); 
			double distance = Math.Sqrt(xDist * xDist + yDist * yDist);
			Direction = new Point(xDist / distance, yDist / distance);
		}

		/* monsters are not bound to a square, but this gives their approximate square
		 * Used for drawing game in the console */
		public Point ApproxSquare()
		{
			int col = (int)Position.X / Square.PIXEL_SIZE;
			int row = (int)Position.Y / Square.PIXEL_SIZE;
			return new Point(col, row);
		}


		
	}
}
