using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
    public enum BuildingType
    {
        TURRET,
        FIRE_OF_LIFE,
        WALL,
        AUTO_HARVESTER_MINE,//COAL AND IRON
        AUTO_HARVESTER_QUARRY,//STONE
        AUTO_HARVESTER_FARM,
        AUTO_HARVESTER_LUMBER_MILL
    }
}
