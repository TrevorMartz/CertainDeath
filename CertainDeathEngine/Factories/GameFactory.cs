using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC.Buildings;
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
            switch(buildingType)
            {
                case BuildingType.TURRET:
                    building = new Turret(World.CurrentTile, position) { Id = GetNextId() };
                    break;
                case BuildingType.WALL:
                    building = new Wall(World.CurrentTile, position) { Id = GetNextId() };
                    break;
                case BuildingType.AUTO_HARVESTER_MINE:
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_MINE, World.Player) { Id = GetNextId() };
                    break;
                case BuildingType.AUTO_HARVESTER_QUARRY:
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_QUARRY, World.Player) { Id = GetNextId() };
                    break;
                case BuildingType.AUTO_HARVESTER_FARM:
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_FARM, World.Player) { Id = GetNextId() };
                    break;
                case BuildingType.AUTO_HARVESTER_LUMBER_MILL:
                    building = new AutoHarvester(World.CurrentTile, position, BuildingType.AUTO_HARVESTER_LUMBER_MILL, World.Player) { Id = GetNextId() };
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
