using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertainDeathEngine.Models.Resources
{
    public class Resource : GameObject
    {
        public int Quantity { get; set; }
        public float RegenRate { get; set; }
		public string Type { get; protected set; }
    }
}
