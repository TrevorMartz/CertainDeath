using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace CertainDeathEngine.Models
{
    [Serializable]
	[JsonObject(MemberSerialization.OptIn)]
    public abstract class GameObject
    {
		[JsonProperty]
        public int Id { get; set; }

		[JsonProperty]
		public Point Position { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public GameObject()
        {
        }

        public GameObject(Point pos)
        {
            this.Position = pos;
        }
    }
}
