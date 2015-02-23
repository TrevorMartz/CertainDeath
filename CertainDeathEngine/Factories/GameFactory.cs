using CertainDeathEngine.Models.NPC;
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
            return new Building() { Id = nextObjectId++ };
        }

        public Monster BuildMonster(string monsterType)
        {
            return new Monster() { Id = nextObjectId++ };
        }
    }
}
