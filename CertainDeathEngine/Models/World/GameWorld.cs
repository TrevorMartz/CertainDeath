using CertainDeathEngine.Models.NPC;
using CertainDeathEngine.Models.NPC.Buildings;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using CertainDeathEngine.Models.User;
using CertainDeathEngine.Models.Resources;

namespace CertainDeathEngine.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class GameWorld
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty]
        public static Dictionary<BuildingType, Cost>
            BuildingCostsForTheWorld = new Dictionary<BuildingType, Cost>()
            {
                { BuildingType.AUTO_HARVESTER_MINE, new Cost() { Costs = { { ResourceType.WOOD, 50 }, { ResourceType.CORN, 25 } } } },
                { BuildingType.AUTO_HARVESTER_QUARRY, new Cost() { Costs = { { ResourceType.IRON, 50 }, { ResourceType.CORN, 25 } } } },
                { BuildingType.AUTO_HARVESTER_FARM, new Cost() { Costs = { { ResourceType.COAL, 50 }, {ResourceType.CORN, 50} } } },
                { BuildingType.AUTO_HARVESTER_LUMBER_MILL, new Cost() { Costs = { { ResourceType.STONE, 50 }, { ResourceType.CORN, 25 } } } },
                { BuildingType.TURRET, new Cost { Costs = { {ResourceType.IRON, 100}, {ResourceType.COAL, 100}, {ResourceType.STONE, 50}, {ResourceType.WOOD, 50}, {ResourceType.CORN, 50} } } },
                { BuildingType.WALL, new Cost { Costs = { {ResourceType.STONE, 100} } } }
            };

        [NonSerialized]
        public Queue<UpdateMessage> Updates;

        [NonSerialized]
        public List<RowColumnPair> SquareClicks;

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
        public Score Score { get; set; }

        public GameWorld()
        {
            // only for use with the json deserializer
            Updates = new Queue<UpdateMessage>();
            SquareClicks = new List<RowColumnPair>();
        }

        public GameWorld(int worldId)
            : this(new Tile(0, 0, null), worldId)
        {
            Updates = new Queue<UpdateMessage>();
            SquareClicks = new List<RowColumnPair>();
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
            SquareClicks = new List<RowColumnPair>();
            Score = new Score
                {
                    FireLevel = 1, WorldId = Id
                };
		}

		public GameWorld(Tile[,] tiles, Tile tile, int worldId)
		{
		    tile.World = this;
            HasEnded = false;
            Player = new Player();
            this.CurrentTile = tile;
			this.Id = worldId;
			Tiles = tiles.Cast<Tile>().ToList();
		    foreach (var t in Tiles)
		    {
		        t.World = this;
            }
            Updates = new Queue<UpdateMessage>();
            SquareClicks = new List<RowColumnPair>();

			var fireOfLife = new FireOfLife(tile);
			tile.AddObject(fireOfLife);
			AddUpdateMessage(new PlaceBuildingUpdateMessage(fireOfLife.Id)
			{
				PosX = fireOfLife.Position.X,
				PosY = fireOfLife.Position.Y,
				Type = fireOfLife.Type.ToString()
			});

            Score = new Score
            {
                FireLevel = 1,
                WorldId = Id
            };
		}

        [OnDeserializing]
        private void OnDeserialize(StreamingContext c)
        {
            lock (this)
            {
                Updates = new Queue<UpdateMessage>();
                SquareClicks = new List<RowColumnPair>();
            }
        }

        public void AddUpdateMessage(UpdateMessage message)
        {
            lock (this.Updates)
            {
                Updates.Enqueue(message);
            }
        }

        public void AddClick(RowColumnPair click)
        {
            lock (this.SquareClicks)
            {
                SquareClicks.Add(click);
            }
        }
    }
}
