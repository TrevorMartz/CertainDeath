using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models
{
    [Serializable]
    public class GameWorld
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public Tile CurrentTile { get; set; }

        public GameWorld()
        {
            // only for use with the json deserializer
        }

        public GameWorld(int worldId) : this(new Tile(), worldId) { }

        public GameWorld(Tile t, int worldId)
        {
            this.CurrentTile = t;
            this.Id = worldId;
        }

        //public void AddObject(GameObject obj)
        //{
        //    this.CurrentTile.AddObject(obj);
        //}
    }
}
