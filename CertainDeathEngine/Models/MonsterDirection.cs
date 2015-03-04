using log4net;
using Newtonsoft.Json;
using System;

namespace CertainDeathEngine.Models
{
	// Its like a java enum
	[JsonObject(MemberSerialization.OptIn)]
	public class MonsterDirection
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private enum XDirection
		{
			LEFT, RIGHT, NONE
		}
		private enum YDirection
		{
			UP, DOWN, NONE
		}
		private enum Direction
		{
			RIGHT, UP_RIGHT, UP, UP_LEFT, LEFT, DOWN_LEFT, DOWN, DOWN_RIGHT
		}

		public readonly double Angle;

		public static readonly MonsterDirection UP = new MonsterDirection(Direction.UP, XDirection.NONE, YDirection.UP);
		public static readonly MonsterDirection DOWN = new MonsterDirection(Direction.DOWN, XDirection.NONE, YDirection.DOWN);
		public static readonly MonsterDirection LEFT = new MonsterDirection(Direction.LEFT, XDirection.LEFT, YDirection.NONE);
		public static readonly MonsterDirection RIGHT = new MonsterDirection(Direction.RIGHT, XDirection.RIGHT, YDirection.NONE);
		public static readonly MonsterDirection UP_LEFT = new MonsterDirection(Direction.UP_LEFT, XDirection.LEFT, YDirection.UP);
		public static readonly MonsterDirection DOWN_LEFT = new MonsterDirection(Direction.DOWN_LEFT, XDirection.LEFT, YDirection.DOWN);
		public static readonly MonsterDirection UP_RIGHT = new MonsterDirection(Direction.UP_RIGHT, XDirection.RIGHT, YDirection.UP);
		public static readonly MonsterDirection DOWN_RIGHT = new MonsterDirection(Direction.DOWN_RIGHT, XDirection.RIGHT, YDirection.DOWN);

		private static double PI_8ths = Math.PI / 8;
		private Direction Dir;
		private XDirection XDir;
		private YDirection YDir;

		[JsonProperty]
		public string X { get { return Enum.GetName(typeof(XDirection), XDir); } }
		[JsonProperty]
		public string Y { get { return Enum.GetName(typeof(YDirection), YDir); } }
		[JsonProperty]
		public string Name { get { return ToString(); } }

		private MonsterDirection (Direction dir, XDirection xdir, YDirection ydir)
		{
			XDir = xdir;
			YDir = ydir;
			Dir = dir;
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
