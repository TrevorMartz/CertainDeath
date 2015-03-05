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
        private long TimeSinceGather { get; set; }
        public AutoHarvester(Tile tile, Point position)
            : base(tile, position)
        {
            Type = BuildingType.AUTOHARVESTER;
            MaxLevel = 5;
            Level = 0;
            Upgrade();
            TimeSinceGather = 0;
        }

        public override void Update(long millis)
        {
            if (HealthPoints <= 0)
            {
                RemoveBuilding();
            }
            if (State == HarvesterState.GATHERING) // todo: this never gets set to gathering
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
            lock (Tile)
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
                        toGather -= s.GatherResource(toGather);
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
                    if(s != null && s.Resource != null /* and resource matches machine type */)
                    {
                        return s;
                    }
                }
            }
            return null;
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
                UpdateCost();
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
        }
    }

    public enum HarvesterState
    {
        GATHERING,
        IDLE
    }
}
