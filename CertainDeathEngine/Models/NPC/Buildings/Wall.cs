using CertainDeathEngine.Models.Resources;
using log4net;
using System;
using System.Windows;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
    public class Wall : Building
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Wall(Tile tile, Point pos) : base(tile, pos)
        {
            Type = BuildingType.WALL;
            MaxLevel = 5;
            Level = 0;
            Upgrade();
        }

        public override void Update(long millis)
        {
            if(HealthPoints <= 0)
            {
                Remove();
            }
        }

        public override void Upgrade()
        {
            if (Level < MaxLevel)
            {
                Level++;
                MaxHealthPoints = 300 * Level;
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
                this.Tile.World.AddUpdateMessage(new UpdateBuildingCostUpdateMessage(this.Id)
                {
                    NewCost = Cost
                });
            }
        }
    }
}
