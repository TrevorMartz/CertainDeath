using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CertainDeathEngine.Models.Resources;
using CertainDeathEngine.Models.World;
using Newtonsoft.Json;

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

		public Resource Resource { get; set; }

		[JsonProperty]
		public string ResourceName { get { return Resource.Type.ToString(); } }

		public bool ShouldSerializeResourceName()
		{
			return Resource != null;
		}

        public int GatherResource(int toTake = 1)
        {
            if(Resource != null)
            {
                int toReturn = Resource.Gather(toTake);
                if(Resource.Quantity == 0)
                {
                    Resource = null;
                }
                return toReturn;
            }
            else
            {
                return 0;
            }
        }
    }
}