﻿using CertainDeathEngine.Models.Resources;
using log4net;
using System;
using System.Windows;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
    class FireOfLife : Building
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [NonSerialized]
        private readonly WorldManager.WorldManager _worldManager = WorldManager.WorldManager.Instance;

        public FireOfLife(Tile tile)
            : base(tile, new Point(Tile.SQUARES_PER_ROW_AND_COLUMN / 2, Tile.SQUARES_PER_ROW_AND_COLUMN / 2), null)
        {
            Type = BuildingType.FIRE_OF_LIFE;
            MaxLevel = 5;
            Level = 0;
            Upgrade();
        }

        public override void Upgrade()
        {
            if (Level < MaxLevel)
            {
                //TODO: Adjust Health Levels and Cost
                Level++;
                Width = Level;
                Height = Level;
                MaxHealthPoints = 500 * Level;
                HealthPoints = MaxHealthPoints;
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

        public void UpdateCost()
        {
            Cost = new Cost();
            //Cost.SetCost(ResourceType.COAL, 10 * Level);
            //Cost.SetCost(ResourceType.CORN, 10 * Level);
            //Cost.SetCost(ResourceType.IRON, 10 * Level);
            //Cost.SetCost(ResourceType.STONE, 10 * Level);
            //Cost.SetCost(ResourceType.WOOD, 10 * Level);
            if (Tile != null)
            {
                this.Tile.World.AddUpdateMessage(new UpdateBuildingCostUpdateMessage(this.Id)
                {
                    NewCost = Cost
                });
            }
        }

        public override void Update(long millis)
        {
            if (HealthPoints <= 0)
            {
                Remove();
                if (Tile != null)
                {
                    this.Tile.World.AddUpdateMessage(new GameOverUpdateMessage(this.Tile.World.Id));
                }
            }
        }
    }
}
