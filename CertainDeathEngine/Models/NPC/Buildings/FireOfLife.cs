using log4net;
using System;
using System.Windows;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
	class FireOfLife : Building
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public FireOfLife(Tile tile)
			: base(tile, new Point(Tile.SQUARES_PER_ROW_AND_COLUMN / 2 , Tile.SQUARES_PER_ROW_AND_COLUMN / 2 ))
		{
            Type = BuildingType.FIREOFLIFE;
            MaxLevel = 5;
            Level = 0;
            Upgrade();
		}

        public override void Upgrade()
        {
            if (Level < MaxLevel)
            {
                Level++;
                Width = Level;
                Height = Level;
                MaxHealthPoints = 100 * Level;
                HealthPoints = MaxHealthPoints;
            }
        }

        public override void Update(long millis)
        {
            return;
            throw new NotImplementedException();
        }
    }
}
