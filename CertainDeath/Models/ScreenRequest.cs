using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CertainDeath.Models
{
    [Serializable]
    public class ScreenRequest
    {
        [JsonProperty]
        public Direction Direction { get; set; }
    }

    public enum Direction
    {
        Above, Below, Left, Right
    }
}