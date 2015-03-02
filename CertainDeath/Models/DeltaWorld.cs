using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CertainDeath.Models
{
    [Serializable]
    public class DeltaWorld
    {
        [JsonProperty]
        public int Id { get; set; }

        [JsonProperty]
        public DeltaTile CurrentTile { get; set; }


    }
}