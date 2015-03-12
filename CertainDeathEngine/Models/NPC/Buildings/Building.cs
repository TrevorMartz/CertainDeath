using log4net;
using Newtonsoft.Json;
using System;
using System.Windows;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Building : Killable, Temporal
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty]
        public string Typename { get { return Type.ToString(); } }

        [JsonProperty]
        public Cost Cost { get; protected set; }

        public int Level { get; protected set; }

        public int MaxLevel { get; protected set; }

        public BuildingType Type { get; protected set; }

        public Point TilePosition { get; set; }

        public Building(Tile tile, Point position)
        {
            Tile = tile;
            Height = Square.PIXEL_SIZE - 1;//TODO: this might be changed later
            Width = Square.PIXEL_SIZE - 1;
            TilePosition = position;
            Position = new Point((position.X * Square.PIXEL_SIZE) + (Square.PIXEL_SIZE / 2), (position.Y * Square.PIXEL_SIZE) + (Square.PIXEL_SIZE / 2));
        }

        public abstract void Update(long millis);

        public abstract void Upgrade();
    }
}
