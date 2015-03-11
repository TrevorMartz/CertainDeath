using log4net;
using Newtonsoft.Json;
using System;

namespace CertainDeathEngine.Models.NPC
{
    [Serializable]
	[JsonObject(MemberSerialization.OptIn)]
    public class Killable : GameObject
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		[JsonProperty]
        public float HealthPoints { get; set; }
		[JsonProperty]
		public int MaxHealthPoints { get; set; }
    }
}
