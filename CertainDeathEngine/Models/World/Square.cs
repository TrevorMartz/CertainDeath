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
	[JsonObject(MemberSerialization.OptIn)]
    public class Square
    {
        public SquareType Type { get;  set; }

		[JsonProperty]
		public string TypeName { get { return Type.ToString(); } }

        public Resource Resource { get; set; }
    }
}