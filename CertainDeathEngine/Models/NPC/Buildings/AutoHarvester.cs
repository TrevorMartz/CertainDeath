using CertainDeathEngine.Models.Resources;
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
        private Player Player { get; set; }
        private long TimeSinceGather { get; set; }
        public AutoHarvester(Tile tile, Point position, BuildingType type, Cost cost, Player p)
            : base(tile, position)
        {
            
            Type = type;
            Cost = cost;
            State = HarvesterState.GATHERING;
            Player = p;
            MaxLevel = 5;
            Level = 0;
            Upgrade();
            TimeSinceGather = 0;
        }

        public override void Update(long millis)
        {
            if (HealthPoints <= 0)
            {
                Remove();
            }
            if (State == HarvesterState.GATHERING)
            {
                TimeSinceGather += millis;
                if (TimeSinceGather >= 1000)
                {
                    long timeToGather = (TimeSinceGather / 1000);
                    Gather((int)(HarvestRate * timeToGather));
                    TimeSinceGather -= timeToGather * 1000;
                }
            }
        }

        private void Gather(int toGather)
        {
            lock (Tile.World)
            {
                while (toGather > 0)
                {
                    RowColumnPair rcp = FindGatherableSquare();
                    Square s = Tile.Squares[rcp.Row, rcp.Column];
                    if (s != null)
                    {
                        this.Tile.World.AddUpdateMessage(new AddResourceToPlayerUpdateMessage(this.Tile.World.Player.Id)
                        {
                            ResourceType = s.Resource.Type.ToString(),
                            Amount = toGather
                        });
                        this.Tile.World.AddUpdateMessage(new RemoveResourceFromSquareUpdateMessage(0) // todo: does a square have an id?
                        {
                            Amount = toGather
                        });
                        if (s.Resource != null)
                        {
                            Tile.World.AddUpdateMessage(new TheSquareNoLongerHasAResourceUpdateMessage(0)
                            {
                                Row = rcp.Row.ToString(),
                                Column = rcp.Column.ToString()
                            });
                        }
                        ResourceType type = s.Resource.Type;
                        int gathered = s.GatherResource(toGather);
                        toGather -= gathered;
                        Player.AddResource(type, gathered);
                    }
                    else
                    {
                        State = HarvesterState.IDLE;
                        this.Tile.World.AddUpdateMessage(new BuildingStateChangeUpdateMessage(this.Id)
                        {
                            State = HarvesterState.IDLE.ToString()
                        });
                        return;
                    }
                }
            }
        }

        private RowColumnPair FindGatherableSquare()
        {
            int row1 = (int)TilePosition.Y - GatherRange;
            if(row1 < 0)
            {
                row1 = 0;
            }
            int col1 = (int)TilePosition.X - GatherRange;
            if(col1 < 0)
            {
                col1 = 0;
            }
            for (int row = row1; row < TilePosition.Y + GatherRange; row++)
            {
                for (int col = col1; col < TilePosition.X + GatherRange; col++)
                {
                    Square s = Tile.Squares[row, col];
                    if(s != null && s.Resource != null && TypeMatches(s.Resource.Type))
                    {
                        return new RowColumnPair(row, col);
                    }
                }
            }
            return null;
        }

        private bool TypeMatches(ResourceType resourceType)
        {
            return (Type == BuildingType.AUTO_HARVESTER_MINE && (resourceType == ResourceType.IRON || resourceType == ResourceType.COAL)) ||
                (Type == BuildingType.AUTO_HARVESTER_QUARRY && resourceType == ResourceType.STONE) ||
                (Type == BuildingType.AUTO_HARVESTER_FARM && resourceType == ResourceType.CORN) ||
                (Type == BuildingType.AUTO_HARVESTER_LUMBER_MILL && resourceType == ResourceType.WOOD);
        }

        public override void Upgrade()
        {
            if (Level < MaxLevel)
            {
                //TODO: Adjust Health Levels, Gather Range, Harvest Rate and Cost
                Level++;
                HarvestRate = Level * 2;
                MaxHealthPoints = 10 * Level;
                HealthPoints = MaxHealthPoints;
                GatherRange = Level * 5;
                UpdateCost();
                if (Tile != null)
                {
                    this.Tile.World.AddUpdateMessage(new UpgradeBuildingUpdateMessage(this.Id)
                    {
                        NewLevel = Level
                    });
                }
            }
        }

        public override void UpdateCost() //TODO: Adjust this if we get to it...
        {
            Cost = new Cost();
            Cost.SetCost(ResourceType.COAL, 10 * Level);
            Cost.SetCost(ResourceType.CORN, 10 * Level);
            Cost.SetCost(ResourceType.IRON, 10 * Level);
            Cost.SetCost(ResourceType.STONE, 10 * Level);
            Cost.SetCost(ResourceType.WOOD, 10 * Level);
            if (Tile != null)
            {
                this.Tile.World.AddUpdateMessage(new UpdateBuildingCostUpdateMessage(this.Id)
                {
                    NewCost = Cost
                });
            }
        }
    }

    public enum HarvesterState
    {
        GATHERING,
        IDLE
    }
}
