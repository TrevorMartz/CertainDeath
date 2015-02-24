using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.NPC
{
    [Serializable]
	[JsonObject(MemberSerialization.OptIn)]
	public class Building : Killable, Temporal
    {
		[JsonProperty]
		public string Type { get { return "Building"; } }

        public float HarvestRate { get; set; }

		public void Update(long millis)
		{
			throw new NotImplementedException();
		}
	}
}
