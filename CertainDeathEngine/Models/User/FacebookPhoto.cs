using log4net;
using Newtonsoft.Json;

namespace CertainDeathEngine.Models.User
{
    public class FacebookPhoto
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty("picture")] // This renames the property to picture.
        public string ThumbnailUrl { get; set; }

        public string Link { get; set; }
    }
}
