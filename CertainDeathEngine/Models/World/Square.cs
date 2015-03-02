using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CertainDeathEngine.Models.Resources;
using CertainDeathEngine.Models.World;
using Newtonsoft.Json;
using CertainDeathEngine.Models.NPC;
using CertainDeathEngine.Models.NPC.Buildings;
using System.Threading;

namespace CertainDeathEngine.Models
{
    [Serializable]
	[JsonObject(MemberSerialization.OptIn)]
    public class Square
    {
		public static readonly int PIXEL_SIZE = 32;
        public SquareType Type { get;  set; }

		[JsonProperty]
		public string TypeName { get { return Type.ToString(); } }

        //using a baton because it is possible to set the resource to null in the lock.
		public Resource Resource { get; set; }

        private object ResourceBaton = new object();

		[JsonProperty]
		public string ResourceName { get { return Resource.Type.ToString(); } }

		public bool ShouldSerializeResourceName()
		{
			return Resource != null;
		}

		// Stores a reference to the building on this square 
		// If there is one
		public Building Building { get; set; }

        public int GatherResource(int toTake = 1)
        {
            if(Resource != null)
            {
                int toReturn;
                lock(ResourceBaton)
                {
                    toReturn = Resource.Gather(toTake);
                    if (Resource.Quantity == 0)
                    {
                        Resource = null;
                    }
                }
                return toReturn;
            }
            else
            {
                return 0;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Square)
            {
                Square s = obj as Square;
                bool equal = false;
                equal = equal || Building != null && s.Building != null;
            }
            return base.Equals(obj);
        }
    }
}