using log4net;
using System;

namespace CertainDeathEngine.Models.NPC
{
    [Serializable]
    public class Killable : GameObject
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public float HealthPoints { get; set; }
        public int MaxHealthPoints { get; set; }
    }
}
