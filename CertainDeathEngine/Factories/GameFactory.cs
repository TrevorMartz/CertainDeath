using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC.Buildings;
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
            switch(buildingType)//fire of life does not have a case because it is only built once, in the constructor of the gameworld.
            {
                case BuildingType.TURRET:
                    building = new Turret(World.CurrentTile, position);
                    break;
                case BuildingType.WALL:
                    building = new Wall(World.CurrentTile, position);
                    break;
                case BuildingType.HARVESTER:
                    building = new AutoHarvester(World.CurrentTile, position);
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
