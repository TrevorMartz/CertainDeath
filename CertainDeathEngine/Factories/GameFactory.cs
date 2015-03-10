using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC.Buildings;
using CertainDeathEngine.Models.Resources;
using log4net;
using System;
using System.Windows;

namespace CertainDeathEngine.Factories
{
    public class GameFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static int _nextObjectId = 1;
        private readonly object _idBaton = new object();

        public GameWorld World { get; protected set; }

        public GameFactory(GameWorld world)
        {
            Log.Debug("Constructing a GameFactory");
            World = world;
        }

        public Building BuildBuilding(BuildingType buildingType, Point position)
        {
            Log.Debug("Trying to build a building of type : " + buildingType + " at position: " + position);
            Building building = null;
            Cost cost = null;
            switch(buildingType)
            {
                case BuildingType.TURRET:
                    building = new Turret(World.CurrentTile, position) { Id = GetNextId() };
                    break;
                case BuildingType.WALL:
                    building = new Wall(World.CurrentTile, position) { Id = GetNextId() };
                    break;
                case BuildingType.AUTO_HARVESTER_MINE:
                    cost = new Cost();
                    cost.Costs[ResourceType.COAL] = 10;
                    cost.Costs[ResourceType.STONE] = 25;
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_MINE, cost, World.Player) { Id = GetNextId() };
                    break;
                case BuildingType.AUTO_HARVESTER_QUARRY:
                    cost = new Cost();
                    cost.Costs[ResourceType.IRON] = 25;
                    cost.Costs[ResourceType.COAL] = 20;
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_QUARRY, cost, World.Player) { Id = GetNextId() };
                    break;
                case BuildingType.AUTO_HARVESTER_FARM:
                    cost = new Cost();
                    cost.Costs[ResourceType.WOOD] = 25;
                    cost.Costs[ResourceType.COAL] = 15;
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_FARM, cost, World.Player) { Id = GetNextId() };
                    break;
                case BuildingType.AUTO_HARVESTER_LUMBER_MILL:
                    cost = new Cost();
                    cost.Costs[ResourceType.STONE] = 20;
                    cost.Costs[ResourceType.IRON] = 15;
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_LUMBER_MILL, cost, World.Player) { Id = GetNextId() };
                    break;
                case BuildingType.FIRE_OF_LIFE: //fire of life does not have a case because it is only built once, in the constructor of the gameworld.
                    Log.Error("You cant build a second Fire of Life");
                    throw new Exception("You cant have another fire of life");
            }
            return building;
        }

		public int GetNextId()
		{
			int id;
			lock (_idBaton)
			{
				id = _nextObjectId++;
			}
			return id;
		}
    }
}
