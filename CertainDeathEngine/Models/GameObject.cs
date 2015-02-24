using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace CertainDeathEngine.Models
{
    [Serializable]
	[JsonObject(MemberSerialization.OptIn)]
    public abstract class GameObject
    {
		[JsonProperty]
        public int Id { get; set; }

		[JsonProperty]
		// The center point of the object
		public virtual Point Position { get { return _Position; } set { _Position = value; CalculateCorners(); } }
		public virtual int Height { get { return _Height; } set { _Width = value; CalculateCorners(); } }
		public virtual int Width { get { return _Width; } set { _Height = value; CalculateCorners(); } }

		// The 4 corners of the game object's rectangle:
		// 0 top left, 1 top right, 2 bottom left, 3 bottom right
		protected Point[] Corners = new Point[4];
		protected Point _Position;
		protected int _Height;
		protected int _Width;
		protected double psuedoRadius;

        public GameObject()
        {
        }

        public GameObject(Point pos)
        {
            this.Position = pos;
        }

		// returns true if a point is inside the game objects rectangle
		// used for monster's collision detection
		public bool ContainsPoint(Point p)
		{
			return Corners[0].X <= p.X && p.X <= Corners[1].X &&
				Corners[0].Y <= p.Y && p.Y <= Corners[2].Y;
		}

		// Make sure this is called after setting the width and height
		protected void CalculateCorners()
		{
			// Calculate the corners
			int halfWidth = Width / 2;
			int halfHeight = Height / 2;
			for (int y = 0; y < 2; y++)
			{
				for (int x = 0; x < 2; x++)
				{
					Corners[y * 2 + x] = new Point(
						Position.X + halfWidth * (x == 0 ? -1 : 1),
						Position.Y + halfHeight * (y == 0 ? -1 : 1)
						);
				}
			}
			psuedoRadius = (halfHeight + halfWidth) / 2; 
		}

		// Gets the distance between two objects based on its center point and psuedoRadius.
		// Since this is a rectangle and not a circle this will be very inacurrate
		// around the corners, but takes only 1 distance comparison
		public double GetFastDistance(GameObject obj)
		{
			return Distance(
				this.Position.X + psuedoRadius,
				obj.Position.X + psuedoRadius,
				this.Position.Y + psuedoRadius,
				obj.Position.Y + psuedoRadius
				);
		}

		// Gets the distance between two objects based on the distance between each of its corners
		// For each corner it will find the distance to each of the other object's corners
		// This will be a total of 16 distance comparisons, but should be ery accurate.
		public double GetAccurateDistance(GameObject obj)
		{

			return 0;
		}

		public double Distance(double x1, double x2, double y1, double y2)
		{
			double xDist = x1 - x2;
			double yDist = y1 - y2;
			return Distance(xDist, yDist);
		}

		public double Distance(double xDist, double yDist)
		{
			return Math.Sqrt(xDist * xDist + yDist * yDist);
		}
    }
}
