using CertainDeathEngine.Models.NPC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public abstract class Building : Killable, Temporal
    {
		[JsonProperty]
		public string Typename { get { return Type.ToString(); } }

        public virtual int Level { get; protected set; }

        public int MaxLevel { get; protected set; }

        public BuildingType Type { get; protected set; }

        public Tile Tile { get; protected set; }

        public Point TilePosition { get; set; }

        public Building(Tile tile, Point position)
        {
            Tile = tile;
            Height = Square.PIXEL_SIZE;
            Width = Square.PIXEL_SIZE;
            TilePosition = position;
            Position = new Point(position.X * Square.PIXEL_SIZE, position.Y * Square.PIXEL_SIZE);
        }

        public abstract void Update(long millis);

        public abstract void Upgrade();

        public void RemoveBuilding()
        {
            Tile.RemoveObject(this);
        }
	}
}
