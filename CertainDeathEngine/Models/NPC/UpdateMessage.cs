using log4net;

namespace CertainDeathEngine.Models.NPC
{
    public class UpdateMessage
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int ObjectId { get; set; }
        public string Data { get; set; }
    }
}
