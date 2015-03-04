using log4net;

namespace CertainDeathEngine.Models.User
{
    public class FacebookPicture
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Url { get; set; }
    }
}
