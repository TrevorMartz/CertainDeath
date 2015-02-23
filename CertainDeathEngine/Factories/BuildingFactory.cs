using CertainDeathEngine.Models.NPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Factories
{
    public class BuildingFactory
    {
        static int nextBuildingId = 1;

        public BuildingFactory()
        {

        }

        public Building BuildBuilding(string buildingType)
        {
            return new Building();
        }
    }
}
