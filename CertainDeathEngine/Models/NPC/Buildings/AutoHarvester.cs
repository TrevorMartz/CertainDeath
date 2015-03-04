using CertainDeathEngine.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
    public class AutoHarvester : Building
    {
        //resources collected per second.
        public int HarvestRate { get; private set; }
        public HarvesterState State { get; set; }
        public Square Square { get; set; }
        public AutoHarvester(Tile tile, Point position) : base(tile, position)
        {
            Move(position);
            MaxLevel = 5;
            Level = 0;
            Upgrade();
        }

        public override void Update(long millis)
        {
            if (State == HarvesterState.GATHERING)
            {
                if (Square.Resource.Quantity <= 0)
                {
                    State = HarvesterState.IDLE;
                    Square = null;
                }
                else
                {
                    Square.GatherResource((int)(HarvestRate * millis));
                }
            }
        }

        public void Move(Tile newTile, Point position)
        {
            Tile = newTile;
            Move(position);
        }

        public void Move(Point position)
        {
            lock (Tile)
            {
                Square = Tile.Squares[(int)position.Y, (int)position.X];
            }
            if(Square.Resource != null && Square.Resource.Quantity > 0)
            {
                State = HarvesterState.GATHERING;
            }
        }

        public override void Upgrade()
        {
            if (Level < MaxLevel)
            {
                Level++;
                HarvestRate = Level;
                MaxHealthPoints = 10 * Level;
                HealthPoints = MaxHealthPoints;
            }
        }
    }

    public enum HarvesterState
    {
        GATHERING,
        IDLE
    }
}
