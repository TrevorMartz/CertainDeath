using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC;
using CertainDeathEngine.Models.NPC.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Factories
{
    public class GameFactory
    {
        static int nextObjectId = 1;
        public GameWorld World { get; protected set; }
        public GameFactory()
        {

        }

        public Building BuildBuilding(BuildingType buildingType, int x, int y)
        {
            Building building = null;
            switch(buildingType)
            {
                case BuildingType.TURRET:
                    //building = new Turret()
                    break;
            }
            return building;
        }

		public int GetNextId()
		{
			int id;
			lock (this)
			{
				id = nextObjectId++;
			}
			return id;
		}
    }
}
