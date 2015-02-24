using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CertainDeathEngine.Models.NPC
{
    [Serializable]
    public class Monster : Killable
    {
		// Get the pixel location of the monster's goal
		public Point Goal { get; set; }

		// The Radius of the monster's colision circle
		public int Radius { get; set; }

		// Monster's movement speed in Pixels per second
		public float Speed { get; set; }

		// Direction to get to goal (As a normalized vector)
		public Point Direction { get; set; }

		public Monster(Point starting, Point goal, int speed)
		{
			Position = starting;
			Goal = goal;
			Speed = speed;
			double xDist = goal.X - Position.X;
			double yDist = goal.Y - Position.Y;
			double distance = Math.Sqrt(xDist * xDist + yDist * yDist);
			Direction = new Point(xDist / distance, yDist / distance);
		}

		// Moves the monster the distance they would go after millisecond have elapsed
		public void Move(int milliseconds)
		{
			Point distance = new Point(Direction.X * Speed * (milliseconds / 1000.0), Direction.Y * Speed * (milliseconds / 1000.0));
			Position = new Point(
				Position.X + distance.X,
				Position.Y + distance.Y);
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
