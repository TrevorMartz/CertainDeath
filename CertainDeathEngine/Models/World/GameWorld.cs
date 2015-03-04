using CertainDeathEngine.Models.NPC;
using CertainDeathEngine.Models.NPC.Buildings;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CertainDeathEngine.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class GameWorld
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [NonSerialized]
        public Queue<UpdateMessage> Updates;
        public long TimeLastSaved { get; set; }
        public long TimeLastQueried { get; set; }
        public long TimeLastUpdated { get; set; }

        [JsonProperty]
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
            Updates = new Queue<UpdateMessage>();
        }

        public GameWorld(int worldId)
            : this(new Tile(0, 0, null), worldId)
        {
            Updates = new Queue<UpdateMessage>();
        }

		public GameWorld(Tile t, int worldId)
		{
		    t.World = this;
            HasEnded = false;
            Player = new Player();
			this.CurrentTile = t;
			this.Id = worldId;
			Tiles = new List<Tile>();
			Tiles.Add(t);
            Updates = new Queue<UpdateMessage>();
		}

		public GameWorld(Tile[,] tiles, Tile tile, int worldId)
		{
		    tile.World = this;
            HasEnded = false;
            Player = new Player();
			this.CurrentTile = tile;
			tile.AddObject(new FireOfLife(tile));
			this.Id = worldId;
			Tiles = tiles.Cast<Tile>().ToList();
		    foreach (var t in Tiles)
		    {
		        t.World = this;
		    }
		    Updates = new Queue<UpdateMessage>();
		}

        [OnDeserializing]
        private void OnDeserialize(StreamingContext c)
        {
            lock (this)
            {
                Updates = new Queue<UpdateMessage>();
            }
        }

        public void AddUpdateMessage(UpdateMessage message)
        {
            lock (this)
            {
                Updates.Enqueue(message);
            }
        }
    }
}
