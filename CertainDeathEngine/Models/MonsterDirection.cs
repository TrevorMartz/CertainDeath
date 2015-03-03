using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models
{
	// Its like a java enum
	public class MonsterDirection
	{
		private enum Direction
		{
			RIGHT, UP_RIGHT, UP, UP_LEFT, LEFT, DOWN_LEFT, DOWN, DOWN_RIGHT
		}

		public readonly double Angle;
		public string Name { get { return ToString(); } }

		public static readonly MonsterDirection UP = new MonsterDirection(Direction.UP);
		public static readonly MonsterDirection DOWN = new MonsterDirection(Direction.DOWN);
		public static readonly MonsterDirection LEFT = new MonsterDirection(Direction.LEFT);
		public static readonly MonsterDirection RIGHT = new MonsterDirection(Direction.RIGHT);
		public static readonly MonsterDirection UP_LEFT = new MonsterDirection(Direction.UP_LEFT);
		public static readonly MonsterDirection DOWN_LEFT = new MonsterDirection(Direction.DOWN_LEFT);
		public static readonly MonsterDirection UP_RIGHT = new MonsterDirection(Direction.UP_RIGHT);
		public static readonly MonsterDirection DOWN_RIGHT = new MonsterDirection(Direction.DOWN_RIGHT);

		private static double PI_8ths = Math.PI / 8;
		private Direction Dir;

		private MonsterDirection (Direction dir)
		{
			Angle = ((int)Dir) * Math.PI / 4;
		}

		public static MonsterDirection GetClosestDirection(double angle)
		{
			if (Math.Abs(UP.Angle - angle) < PI_8ths)
			{
				return UP;
			}
			if (Math.Abs(LEFT.Angle - angle) < PI_8ths)
			{
				return LEFT;
			}
			if (Math.Abs(RIGHT.Angle - angle) < PI_8ths)
			{
				return RIGHT;
			}
			if (Math.Abs(UP_RIGHT.Angle - angle) < PI_8ths)
			{
				return UP_RIGHT;
			}
			if (Math.Abs(UP_LEFT.Angle - angle) < PI_8ths)
			{
				return UP_LEFT;
			}
			if (Math.Abs(DOWN_RIGHT.Angle - angle) < PI_8ths)
			{
				return DOWN_RIGHT;
			}
			if (Math.Abs(DOWN_LEFT.Angle - angle) < PI_8ths)
			{
				return DOWN_LEFT;
			}
			return DOWN;
		}

		public override string ToString()
		{
			return Enum.GetName(typeof(Direction), Dir);
		}
	}
}
