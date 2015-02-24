using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertainDeathEngine.Models.Resources
{
    [Serializable]
    public class Resource
    {
        public int Quantity { get; set; }
		public ResourceType Type { get; set; }

        public Resource(ResourceType type, int quantity)
        {
            this.Type = type;
            this.Quantity = quantity;
        }
    }
}
