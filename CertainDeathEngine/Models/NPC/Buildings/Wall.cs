using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    public class Wall : Building
    {
        public Wall(Tile tile, Point pos) : base(tile, pos)
        {
            Type = BuildingType.WALL;
            MaxLevel = 5;
            Level = 0;
            Upgrade();
            
        }

        public override void Update(long millis)
        {
            return;
            throw new NotImplementedException();
        }

        public override void Upgrade()
        {
            if (Level < MaxLevel)
            {
                Level++;
                MaxHealthPoints = 100 * Level;
                HealthPoints = MaxHealthPoints;
            }
        }
    }
}
