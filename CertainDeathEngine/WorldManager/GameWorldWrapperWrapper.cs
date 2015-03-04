using log4net;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CertainDeathEngine.WorldManager
{
    [Table("Worlds")]
    public class GameWorldWrapperWrapper
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Key]
        public int Id { get; set; }
        public int WorldId { get; set; }
        public virtual GameWorldWrapper Worlds { get; set; }
    }
}
