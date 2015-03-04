using log4net;
using System;
using System.Windows;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
    public class AutoHarvester : Building
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //resources collected per second.
        public int HarvestRate { get; private set; }
        public int GatherRange { get; set; }
        public HarvesterState State { get; set; }
        private long TimeSinceGather { get; set; }
        public AutoHarvester(Tile tile, Point position)
            : base(tile, position)
        {
            Type = BuildingType.HARVESTER;
            //Move(position);
            MaxLevel = 5;
            Level = 0;
            Upgrade();
            TimeSinceGather = 0;
        }

        public override void Update(long millis)
        {
            if (State == HarvesterState.GATHERING)
            {
                TimeSinceGather += millis;
                if (TimeSinceGather >= 1000)
                {
                    long timeToGather = (TimeSinceGather / 1000);
                    //Console.WriteLine("TimeSinceGather: " + TimeSinceGather);
                    //Console.WriteLine("timeToGather: " + timeToGather);
                    Gather((int)(HarvestRate * timeToGather));
                    TimeSinceGather -= timeToGather * 1000;
                    //Console.WriteLine("TimeSinceGather after gather: " + TimeSinceGather + "\n");
                }
            }
        }

        private void Gather(int toGather)
        {
            lock (Tile)
            {
                while (toGather > 0)
                {
                    Square s = FindGatherableSquare();
                    if (s != null)
                    {
                        toGather -= s.GatherResource(toGather);
                    }
                    else
                    {
                        State = HarvesterState.IDLE;
                        return;
                    }
                }
            }
        }

        private Square FindGatherableSquare()
        {
            for (int row = (int)TilePosition.Y - GatherRange; row < TilePosition.Y + GatherRange; row++)
            {
                for (int col = (int)TilePosition.X - GatherRange; col < TilePosition.X + GatherRange; col++)
                {
                    Square s = Tile.Squares[row, col];
                    if(s != null && s.Resource != null /* and resource matches machine type */)
                    {
                        return s;
                    }
                }
            }
            return null;
        }

        public void Move(Tile newTile, Point position)
        {
            Tile = newTile;
            Move(position);
        }

        public void Move(Point position)
        {
            //lock (Tile)
            //{
            //    Square = Tile.Squares[(int)position.Y, (int)position.X];
            //}
            //if(Square.Resource != null && Square.Resource.Quantity > 0)
            //{
            //    State = HarvesterState.GATHERING;
            //}
        }

        public override void Upgrade()
        {
            if (Level < MaxLevel)
            {
                Level++;
                HarvestRate = Level;
                MaxHealthPoints = 10 * Level;
                HealthPoints = MaxHealthPoints;
                GatherRange = Level;
            }
        }
    }

    public enum HarvesterState
    {
        GATHERING,
        IDLE
    }
}
