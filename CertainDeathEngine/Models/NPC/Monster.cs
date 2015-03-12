using CertainDeathEngine.Models.NPC.Buildings;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Windows;

namespace CertainDeathEngine.Models.NPC
{
    [Serializable]
	[JsonObject(MemberSerialization.OptIn)]
    public class Monster : Killable, Temporal
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
		public Point DirectionVector { get; private set; }

		[JsonProperty]
		// Approximate direction, rounded to the nearest of the 8
		public MonsterDirection Direction { get; private set; }

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

		[JsonProperty]
		public String Status { get { return Enum.GetName(typeof(MonsterState), State); } }

		// The building the monster is attacking
		private Building Attacking { get; set; }

		// The three leading corners in the direction the monster is walking
		private Point[] LeadingCorners = new Point[3];
		// The time in milliseconds it takes for this monster to walk half a sqaure
		private int MillisPerHalfSquare;

		private MonsterName MonsterName;

		[JsonProperty]
		public string Name { get { return Enum.GetName(typeof(MonsterName), MonsterName); } }

		public Monster(Tile t, Point starting, Point goal, int speed, MonsterName name = MonsterName.STONE_GOLEM)
		{
			MaxHealthPoints = (new Random().Next(3)+1) * 50;
			HealthPoints = MaxHealthPoints;
			Tile = t;
			Position = starting;
			Goal = goal;
			Speed = speed;
			MillisPerHalfSquare = (int)((HALF_SQUARE / speed) * 1000);
			State = MonsterState.WALKING; // Go, find the fire, my minon!
			_Height = Square.PIXEL_SIZE;
			_Width = Square.PIXEL_SIZE;
			MonsterName = name;
			CalculateDirection();
		}
		public void Update(long millis)
		{
            // check the clicks

            var monsterSquare = new RowColumnPair(GameObject.ApproxSquare(Position));
		    lock (Tile.World.SquareClicks)
		    {
		        bool toRemoveSomthing = Tile.World.SquareClicks.Contains(monsterSquare);

		        if (toRemoveSomthing)
		        {
                    Tile.World.SquareClicks.Remove(monsterSquare);
                    this.HealthPoints -= 1000;
                    this.Tile.World.AddUpdateMessage(new HealthUpdateMessage(this.Id)
                    {
                        HealthPoints = this.HealthPoints
                    });

		        }
		    }

		    if (HealthPoints <= 0)
		    {
		        this.Remove();
		    }

            //millis = millis * 1000;
            //Trace.WriteLine("Monster " + Id + " is updating");
			if (State == MonsterState.ATTACKING)
			{
				if (Attacking.HealthPoints <= 0)
				{
					// someone else killed it, do nothing this step, then start walking
					// If we can sell buildings there could be a problem here
					State = MonsterState.WALKING;

                    this.Tile.World.AddUpdateMessage(new MonsterStateChangeUpdateMessage(this.Id)
                    {
                        State = MonsterState.WALKING.ToString()
                    });
					Attacking = null;
				}
				else
				{
					Attack(millis);
				}
			}
			else if (State == MonsterState.WALKING)
			{
				Building colide = FindAttackableBuilding(millis);
				if (colide != null)
				{
					Attacking = colide;
                    State = MonsterState.ATTACKING;
                    this.Tile.World.AddUpdateMessage(new MonsterStateChangeUpdateMessage(this.Id)
                    {
                        State = MonsterState.ATTACKING.ToString()
                    });
				}
			}
		}

		private Building FindAttackableBuilding(long millis)
		{
			Point XYdist = GetDistanceOverTime(millis);
			double distanceCanGo = Distance(XYdist.X, XYdist.Y);
			bool somethingIsClose = false;
            lock (Tile.World)
            {
                for (int i = 0; i < Tile.Buildings.Count && !somethingIsClose; i++)
                {
                    double dist = GetFastDistance(Tile.Buildings[i]);
                    somethingIsClose = dist <= distanceCanGo;
                }
            }

			if (somethingIsClose)
			{
				// shoot a ray out of each significant corner and find the closest building
				// move to it and attack
				Collision[] collis = new Collision[LeadingCorners.Length];
				for (int i = 0; i < collis.Length; i++)
				{
					collis[i] = ShootRay(LeadingCorners[i], XYdist, millis);
				}
				
				Collision closestCollision = null;
				foreach (Collision c in collis)
				{
					if (closestCollision == null || c.Distance < closestCollision.Distance )
					{
						closestCollision = c;
					}
				}
				if (closestCollision.Hit == null)
				{
					Move(XYdist);
				}
				else
				{
					double ratioTraveled = closestCollision.Distance / distanceCanGo;
					Move(closestCollision.DistancePoint);//XYdist.X * ratioTraveled, XYdist.Y * ratioTraveled));
					return closestCollision.Hit;
				}
			}
			else
			{
				Move(XYdist);
			}
			return null;
		}

		private Collision ShootRay(Point startingPoint, Point maxDist, long millis)
		{
			Point dist = new Point(Math.Abs(maxDist.X), Math.Abs(maxDist.Y));
			double xSpeed = Math.Abs(DirectionVector.X);
			double ySpeed = Math.Abs(DirectionVector.Y);
			Building hit = null;
			Point pos = startingPoint;
			System.Drawing.Point squarePos = ApproxSquare();
			//while (hit == null &&
			//	Math.Abs(startingPoint.X - pos.X) < dist.X &&
			//	Math.Abs(startingPoint.Y - pos.Y) < dist.Y)
			//{
				double xDistToNextSquare = Square.PIXEL_SIZE * (DirectionVector.X < 0 ? -1 : 0) + squarePos.X * Square.PIXEL_SIZE - pos.X; //(squarePos.X + (Direction.X < 0 ? -1 : 1)) * Square.PIXEL_SIZE - pos.X;
				double yDistToNextSquare = Square.PIXEL_SIZE * (DirectionVector.Y < 0 ? -1 : 1) + squarePos.Y * Square.PIXEL_SIZE - pos.Y; //(squarePos.Y + (Direction.Y < 0 ? -1 : 1)) * Square.PIXEL_SIZE - pos.Y;
				double xTime = Math.Abs(xDistToNextSquare) / xSpeed;
				double yTime = Math.Abs(yDistToNextSquare) / ySpeed;
				if (xTime < yTime && xTime < millis)
				{
					pos = new Point(
						pos.X + xDistToNextSquare,
						pos.Y + DirectionVector.Y * xTime);
					squarePos.X = squarePos.X + (DirectionVector.X > 0 ? 1 : -1);
				}
				else if (yTime < xTime && yTime < millis)
				{
					pos = new Point(
						pos.X + DirectionVector.X * yTime,
						pos.Y + yDistToNextSquare);
					squarePos.Y = squarePos.Y + (DirectionVector.Y > 0 ? 1 : -1);
				}
				else if(yTime == xTime)
				{
					if (yTime < millis && xTime < millis)
					{
						pos = new Point(
							pos.X + xDistToNextSquare,
							pos.Y + yDistToNextSquare);
						squarePos.X = squarePos.X + (DirectionVector.X > 0 ? 1 : -1);
						squarePos.Y = squarePos.Y + (DirectionVector.Y > 0 ? 1 : -1);
					}
				}
                lock (Tile.World)
                {
                    hit = Tile.Squares[squarePos.Y, squarePos.X].Building;
                }
			//}
				return new Collision(new Point(pos.X - startingPoint.X,
						pos.Y - startingPoint.Y) ) { Hit = hit };
		}

		private void Attack(long millis)
		{
			float damage = Damage * (millis / 1000.0f);
			Attacking.HealthPoints -= damage;
            this.Tile.World.AddUpdateMessage(new HealthUpdateMessage(Attacking.Id)
            {
                HealthPoints = Attacking.HealthPoints
            });
			if (Attacking.HealthPoints <= 0)
			{
                Attacking.Remove();
                State = MonsterState.WALKING;
                Attacking = null;
			}
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

			if(Tile == Tile.World.CurrentTile)
				this.Tile.World.AddUpdateMessage(new MoveUpdateMessage(this.Id)
				{
					MoveX = Position.X,
					MoveY = Position.Y
				});

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
				DirectionVector.X * Speed * (milliseconds / 1000.0),
				DirectionVector.Y * Speed * (milliseconds / 1000.0));
		}

		private void MoveToTile(Tile tile, Point positionChange)
		{
            lock (Tile.World)
            {
                if (tile == null)
                {
                    // Then the monster has walked off the world.
                    // Either it has killed the fire of life, the game
                    // didn't end, and he kept walking.
                    // or he is trying to get to the fire of life and walked
                    // through a null tile. 
                    Tile.RemoveObject(this);
                    this.Tile.World.AddUpdateMessage(new RemoveUpdateMessage(this.Id));
                }
                else
                {
                    Tile.RemoveObject(this);
					if(this.Tile == Tile.World.CurrentTile)
						this.Tile.World.AddUpdateMessage(new RemoveUpdateMessage(this.Id));
                    Tile = tile;
                    tile.AddObject(this);
                    Position = new Point(
                        Position.X + positionChange.X,
                        Position.Y + positionChange.Y);
					if (this.Tile == Tile.World.CurrentTile)
						this.Tile.World.AddUpdateMessage(new PlaceMonsterUpdateMessage(Id)
						{
							PosX = Position.X,
							PosY = Position.Y,
							Type = Name,
							Direction = Direction,
							State = Status
						});

					//this.Tile.World.AddUpdateMessage(new MoveUpdateMessage(this.Id)
					//{
					//	MoveX = Position.X,
					//	MoveY = Position.Y
					//});
                    //todo: i dont know how to send a move update, so I will send a whole world update insteac
                    //this.Tile.World.AddUpdateMessage(new WorldUpdateMessage());
                }
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
			DirectionVector = new Point(xDist / distance, yDist / distance);
			CalcApproxDirection();
			//Rotation = Math.Atan2(Direction.Y, Direction.X);
		}

		private void CalcApproxDirection()
		{
			if (DirectionVector.X == 0)
				if (DirectionVector.Y > 0)
					Direction = MonsterDirection.DOWN;
				else
					Direction = MonsterDirection.UP;
			else if (DirectionVector.Y == 0)
				if (DirectionVector.X > 0)
					Direction = MonsterDirection.RIGHT;
				else
					Direction = MonsterDirection.LEFT;
			else
			{
				double riseOverRun = -DirectionVector.Y / DirectionVector.X;
				double angleRadians = Math.Atan(riseOverRun);
				if (DirectionVector.X > 0 && DirectionVector.Y > 0)
					angleRadians += Math.PI * 2;
				else if (DirectionVector.X < 0)
					angleRadians += Math.PI;
				Direction = MonsterDirection.GetClosestDirection(angleRadians);
			} 
		}


		private void Find3SignificantCorners()
		{
			// calculate the distances between each corner and the direction
			double[] distances = new double[4];
			for (int i = 0; i < 4; i++)
			{
				distances[i] = Distance(Position.X + DirectionVector.X, Corners[i].X,
					Position.Y + DirectionVector.Y, Corners[i].Y);
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

		private class Collision
		{
			public Building Hit { get; set; }
			public double Distance { get; set; }
			public Point DistancePoint { get; set; }

			public Collision(Point dist)
			{
				DistancePoint = dist;
				Distance = GameObject.Distance(dist.X, dist.Y);
			}
		}
	}

    public enum MonsterState
    {
        WALKING, ATTACKING, DYING
    }

	public enum MonsterName
	{
		BASAL_GOLEM,
		BANSHEE,
		BRIGAND,
		CLERIC,
		PEASANT,
		DRUID,
		FURY,
		GARGOYLE,
		GHOUL,
		GNOME,
		STONE_GOLEM,
		GORGON,
		GRIFFEN,
		RANGER,
		WARRIOR,
		WIZARD,
		WOLF,
		ZOMBIE
	}
}
