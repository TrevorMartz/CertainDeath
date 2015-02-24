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
		private static int HALF_SQUARE = Square.PIXEL_SIZE / 2;
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
		public Point Direction { get; private set; }

		// Monster needs to know where they are so they
		// can go towards the FireOfLife
		public Tile Tile { get; private set; }

		// Monster's damage per second
		// This will be used when it is in contact with a building
		// We can improve this later with attack speed and attack damage
		// if we end up having animaitions.
		public float Damage { get; set; }

		// Override these so the direction is also calculated
		public override Point Position
		{
			get { return _Position; }
			set
			{
				_Position = value;
				CalculateCorners();
				Find3SignificantCorners();
			}
		}
		public override int Width { get { return _Width; } 
			set 
			{
				_Width = value;
				CalculateCorners();
				Find3SignificantCorners();
			} }
		public override int Height { get { return _Height; } 
			set
			{
				_Height = value;
				CalculateCorners();
				Find3SignificantCorners();
			} }

		// Monsters's current state {WALKING, ATTAKING, or DYING}
		private MonsterState State { get; set; }

		// The building the monster is attacking
		private Building Attacking { get; set; }

		// The three leading corners in the direction the monster is walking
		private Point[] LeadingCorners = new Point[3];
		// The time in milliseconds it takes for this monster to walk half a sqaure
		private int MillisPerHalfSquare;

		public Monster(Tile t, Point starting, Point goal, int speed)
		{
			Tile = t;
			Position = starting;
			Goal = goal;
			Speed = speed;
			MillisPerHalfSquare = (int)((HALF_SQUARE / speed) * 1000);
			State = MonsterState.WALKING; // Go, find the fire, my minon!
			_Height = Square.PIXEL_SIZE;
			_Width = Square.PIXEL_SIZE;
			CalculateDirection();
		}
		public void Update(long millis)
		{
			if (State == MonsterState.ATTACKING)
			{
				if (Attacking.HealthPoints >= 0)
				{
					// someone else killed it, do nothing this step, then start walking
					// If we can sell buildings there could be a problem here
					State = MonsterState.WALKING;
					Attacking = null;
				}
				else
				{
					Attack(millis);
				}
			}
			else if (State == MonsterState.WALKING)
			{
				Building colide = WalkUntilYouHitSomethingOrRunOutOfTimeThenReturnWhatYouRanInto(millis);
				if (colide != null)
				{
					Attacking = colide;
					State = MonsterState.ATTACKING;
				}
			}
		}

		private Building WalkUntilYouHitSomethingOrRunOutOfTimeThenReturnWhatYouRanInto(long millis)
		{
			//for now will will assume that monsters slide toward their target and don't turn.
			//int locationsToCheck = (int)millis / MillisPerHalfSquare;
			//for (int i = 0; i < locationsToCheck - 1; i++)
			//{
			//	int pixDist = (i + 1) * HALF_SQUARE;
			//	Point distance = new Point(
			//		pixDist * Direction.X,
			//		pixDist * Direction.Y
			//		);
			//	Building b = null;
			//	for(int corn = 0; corn < LeadingCorners.Length && b == null; corn++)// (Point corner in LeadingCorners)
			//	{
			//		b = GetBuildingAtPoint(
			//			new Point(
			//				LeadingCorners[corn].X + distance.X,
			//				LeadingCorners[corn].Y + distance.Y));
			//		if (b == null)
			//		{
						
			//		}
			//		else 
			//		{
			//			return b;
			//		}
			//	}
			//}

			Point XYdist = GetDistanceOverTime(millis);
			double distanceCanGo = Distance(XYdist.X, XYdist.Y);
			bool somethingIsClose = false;
			for (int i = 0; i < Tile.Buildings.Count && ! somethingIsClose; i++)
			{
				double dist = GetFastDistance(Tile.Buildings[i]);
				somethingIsClose = dist <= distanceCanGo;
			}

			if (somethingIsClose)
			{
				// more accurate cals
				int blakeisdumb = 0;
			}
			else
			{
				Move(XYdist);
			}
			return null;
		}

		private Building GetBuildingAtPoint(Point p)
		{
			foreach (Building b in Tile.Buildings)
			{
				if (b.ContainsPoint(p))
				{
					return b;
				}
			}
			return null;
		}

		private void Attack(long millis)
		{
			float damage = Damage * (millis / 1000.0f);
			Attacking.HealthPoints -= damage;
		}

		// Moves the monster the distance they would go after millisecond have elapsed
		// This will need to return something or call a callback to notify the front end
		// if a monster moves from one tile to another. (if we aren't sending the whole game)
		public void Move(Point distance)
		{
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

		// calculate how far the monster will move
		public Point GetDistanceOverTime(long milliseconds)
		{
			return new Point(
				Direction.X * Speed * (milliseconds / 1000.0),
				Direction.Y * Speed * (milliseconds / 1000.0));
		}

		private void MoveToTile(Tile tile, Point positionChange)
		{
			if (tile == null)
			{
				// Then the monster has walked off the world.
				// Either it has killed the fire of life, the game
				// didn't end, and he kept walking.
				// or he is trying to get to the fire of life and walked
				// through a null tile. 
				Tile.RemoveObject(this);
			}
			else {
				Tile.RemoveObject(this);
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
			double distance = Distance(xDist, yDist);
			Direction = new Point(xDist / distance, yDist / distance);
			//Rotation = Math.Atan2(Direction.Y, Direction.X);
		}


		private void Find3SignificantCorners()
		{
			// calculate the distances between each corner and the direction
			double[] distances = new double[4];
			for (int i = 0; i < 4; i++)
			{
				distances[i] = Distance(Position.X + Direction.X, Corners[i].X,
					Position.Y + Direction.Y, Corners[i].Y);
			}

			// find the corner with the biggest distance
			double largestDistance = int.MinValue;
			int largestIndex = 0;
			for (int i = 0; i < 4; i++)
			{
				if (distances[i] > largestDistance)
				{
					largestDistance = distances[i];
					largestIndex = i;
				}
			}

			// add all corners to the LeadingCorners array, 
			// except for the corner with the biggest difference to the direction
			int added = 0;
			for (int i = 0; i < 4; i++)
			{
				if (i != largestIndex)
				{
					LeadingCorners[added++] = Corners[i];
				}
			}
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
