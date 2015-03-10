using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC.Buildings;
using CertainDeathEngine.Models.Resources;
using log4net;
using System.Windows;

namespace CertainDeathEngine.Factories
{
    public class GameFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static int nextObjectId = 1;
        object idBaton = new object();
        public GameWorld World { get; protected set; }
        public GameFactory(GameWorld world)
        {
            World = world;
        }

        public Building BuildBuilding(BuildingType buildingType, Point position)
        {
            Building building = null;
            Cost cost = null;
            switch(buildingType)//fire of life does not have a case because it is only built once, in the constructor of the gameworld.
            {
                case BuildingType.TURRET:
                    building = new Turret(World.CurrentTile, position);
                    break;
                case BuildingType.WALL:
                    building = new Wall(World.CurrentTile, position);
                    break;
                case BuildingType.AUTO_HARVESTER_MINE:
                    cost = new Cost();
                    cost.Costs[ResourceType.COAL] = 10;
                    cost.Costs[ResourceType.STONE] = 25;
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_MINE, cost, World.Player);
                    break;
                case BuildingType.AUTO_HARVESTER_QUARRY:
                    cost = new Cost();
                    cost.Costs[ResourceType.IRON] = 25;
                    cost.Costs[ResourceType.COAL] = 20;
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_QUARRY, cost, World.Player);
                    break;
                case BuildingType.AUTO_HARVESTER_FARM:
                    cost = new Cost();
                    cost.Costs[ResourceType.WOOD] = 25;
                    cost.Costs[ResourceType.COAL] = 15;
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_FARM, cost, World.Player);
                    break;
                case BuildingType.AUTO_HARVESTER_LUMBER_MILL:
                    cost = new Cost();
                    cost.Costs[ResourceType.STONE] = 20;
                    cost.Costs[ResourceType.IRON] = 15;
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_LUMBER_MILL, cost, World.Player);
                    break;
            }
            return building;
        }

		public int GetNextId()
		{
			int id;
			lock (idBaton)
			{
				id = nextObjectId++;
			}
			return id;
		}
    }
}
