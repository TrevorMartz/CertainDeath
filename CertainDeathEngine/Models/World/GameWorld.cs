using CertainDeathEngine.Models.NPC;
using CertainDeathEngine.Models.NPC.Buildings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models
{
    [Serializable]
    public class GameWorld
    {
        public Player Player { get; set; }

        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public Tile CurrentTile { get; set; }

		public  List<Tile> Tiles { get; private set; }

        public bool HasEnded { get; set; }

        public GameWorld()
        {
            // only for use with the json deserializer
        }

        public GameWorld(int worldId) : this(new Tile(0, 0), worldId) { }

		public GameWorld(Tile t, int worldId)
		{
            HasEnded = false;
            Player = new Player();
			this.CurrentTile = t;
			this.Id = worldId;
			Tiles = new List<Tile>();
			Tiles.Add(t);
		}

		public GameWorld(Tile[,] tiles, Tile tile, int worldId)
        {
            HasEnded = false;
            Player = new Player();
			this.CurrentTile = tile;
			tile.AddObject(new FireOfLife(tile));
			this.Id = worldId;
			Tiles = tiles.Cast<Tile>().ToList();
		}
    }
}
