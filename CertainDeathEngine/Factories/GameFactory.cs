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

        public GameFactory()
        {

        }

        public Building BuildBuilding(string buildingType)
        {
			return new Building() { Id = GetNextId()};
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
