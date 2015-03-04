using log4net;
using System;
using System.Windows;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
    public class Wall : Building
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
