using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CertainDeath.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class DeltaTile
    {
        public Square[,] NewSquares { get; set; }
        public Square[,] OldSquares { get; set; }

        //[JsonProperty]
        public Square[,] Squares()
        {
            for (int i = 0; i < NewSquares.Length; ++i)
            {

            }
            return null;
        }

        [JsonProperty]
        public List<Monster> Monsters { get; set; }
    }
}