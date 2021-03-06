﻿using CertainDeathEngine.Models.Resources;
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
        public AutoHarvester(Tile tile, Point position, BuildingType type, Player p, Cost cost)
            : base(tile, position, cost)
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
            TimeSinceGather += millis;
            if (HealthPoints <= 0)
            {
                Remove();
            }
            if (State == HarvesterState.GATHERING)
            {
                if (TimeSinceGather >= 1000)
                {
                    long timeToGather = (TimeSinceGather / 1000);
                    int toGather = (int)(HarvestRate * timeToGather);
                    Gather(toGather);
                    TimeSinceGather -= timeToGather * 1000;
                }
            }
            else
            {
                if (TimeSinceGather >= 5000)
                {
                    TimeSinceGather -= 5000;
                    ResourceType toGather = ResourceType.CORN;//only given default because i have to, will ALWAYS be set in the switch.
                    switch (Type)
                    {
                        case BuildingType.AUTO_HARVESTER_FARM:
                            toGather = ResourceType.CORN;
                            break;
                        case BuildingType.AUTO_HARVESTER_LUMBER_MILL:
                            toGather = ResourceType.WOOD;
                            break;
                        case BuildingType.AUTO_HARVESTER_MINE:
                            toGather = (RandomGen.Random.Next(2) == 0 ? ResourceType.COAL : ResourceType.IRON);
                            break;
                        case BuildingType.AUTO_HARVESTER_QUARRY:
                            toGather = ResourceType.STONE;
                            break;
                    }
                    Player.AddResource(toGather, 1);
                    Tile.World.Score.AddResource(toGather, 1);
                    this.Tile.World.AddUpdateMessage(new AddResourceToPlayerUpdateMessage(this.Tile.World.Player.Id)
                    {
                        ResourceType = toGather.ToString(),
                        Amount = 1
                    });
                    if (Tile == Tile.World.CurrentTile)
                        this.Tile.World.AddUpdateMessage(new RemoveResourceFromSquareUpdateMessage(0)
                        {
                            Amount = 1,
                            Row = TilePosition.Y.ToString(),
                            Column = TilePosition.X.ToString()
                        });
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
                    if (rcp != null)
                    {
                        Square s = Tile.Squares[rcp.Row, rcp.Column];
                        bool current = Tile == Tile.World.CurrentTile;
                        if (s != null && s.Resource != null)
                        {
							this.Tile.World.AddUpdateMessage(new AddResourceToPlayerUpdateMessage(this.Tile.World.Player.Id)
																{
																	ResourceType = s.Resource.Type.ToString(),
																	Amount = toGather
																});
							if(current)
								this.Tile.World.AddUpdateMessage(new RemoveResourceFromSquareUpdateMessage(0)
																{
																	Amount = toGather,
																	Row = rcp.Row.ToString(),
																	Column = rcp.Column.ToString()
																});

                            ResourceType type = s.Resource.Type;
                            int gathered = s.GatherResource(toGather);
                            toGather -= gathered;
                            Player.AddResource(type, gathered);
                            Tile.World.Score.AddResource(type, gathered);
                            if (s.Resource == null && current)
                            {
                                Tile.World.AddUpdateMessage(new TheSquareNoLongerHasAResourceUpdateMessage(0)
                                {
                                    Row = rcp.Row.ToString(),
                                    Column = rcp.Column.ToString()
                                });
                            }
                        }
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
            int minrow = Math.Max(0, (int)TilePosition.Y - GatherRange);
            int maxrow = Math.Min((int)TilePosition.Y + GatherRange, 20);
            int mincol = Math.Max(0, (int)TilePosition.X - GatherRange);
            int maxcol = Math.Min((int)TilePosition.X + GatherRange, 20);

            for (int row = minrow; row < maxrow; row++)
            {
                for (int col = mincol; col < maxcol; col++)
                {
                    Square s = Tile.Squares[row, col];
                    if (s != null && s.Resource != null && TypeMatches(s.Resource.Type))
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
                MaxHealthPoints = 200 * Level;
                HealthPoints = MaxHealthPoints;
                GatherRange = Level * 3;
                if (Tile != null)
                {
                    this.Tile.World.AddUpdateMessage(new UpgradeBuildingUpdateMessage(this.Id)
                    {
                        NewLevel = Level
                    });
                }
            }
        }
    }

    public enum HarvesterState
    {
        GATHERING,
        IDLE
    }
}
