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
        public AutoHarvester(Tile tile, Point position, BuildingType type, Player p)
            : base(tile, position)
        {
            
            Type = type;
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
                    Square s = FindGatherableSquare();
                    if (s != null)
                    {
                        this.Tile.World.AddUpdateMessage(new AddResourceToPlayerUpdateMessage()
                        {
                            ObjectId = this.Tile.World.Player.Id,
                            ResourceType = s.Resource.Type.ToString(),
                            Amount = toGather
                        });
                        this.Tile.World.AddUpdateMessage(new RemoveResourceFromSquareUpdateMessage()
                        {
                            ObjectId = 0, // todo: does a square have an id?
                            Amount = toGather
                        });
                        ResourceType type = s.Resource.Type;
                        int gathered = s.GatherResource(toGather);
                        toGather -= gathered;
                        Player.AddResource(type, gathered);
                    }
                    else
                    {
                        State = HarvesterState.IDLE;
                        this.Tile.World.AddUpdateMessage(new BuildingStateChangeUpdateMessage()
                        {
                            ObjectId = this.Id,
                            State = HarvesterState.IDLE.ToString()
                        });
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
                    if(s != null && s.Resource != null && TypeMatches(s.Resource.Type))
                    {
                        return s;
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
                Level++;
                HarvestRate = Level * 2;
                MaxHealthPoints = 10 * Level;
                HealthPoints = MaxHealthPoints;
                GatherRange = Level * 5;
                UpdateCost();
                if (Tile != null)
                {
                    this.Tile.World.AddUpdateMessage(new UpgradeBuildingUpdateMessage()
                    {
                        ObjectId = this.Id,
                        NewLevel = Level
                    });
                }
            }
        }

        public override void UpdateCost()
        {
            Cost = new Cost();
            Cost.SetCost(ResourceType.COAL, 10 * Level);
            Cost.SetCost(ResourceType.CORN, 10 * Level);
            Cost.SetCost(ResourceType.IRON, 10 * Level);
            Cost.SetCost(ResourceType.STONE, 10 * Level);
            Cost.SetCost(ResourceType.WOOD, 10 * Level);
            if (Tile != null)
            {
                this.Tile.World.AddUpdateMessage(new UpdateBuildingCostUpdateMessage()
                {
                    ObjectId = this.Id,
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
