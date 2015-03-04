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
            this.Tile.World.AddUpdateMessage(new RemoveUpdateMessage()
            {
                ObjectId = this.Id
            });
            Tile.RemoveObject(this);
        }

        public abstract void UpdateCost();
    }
}
