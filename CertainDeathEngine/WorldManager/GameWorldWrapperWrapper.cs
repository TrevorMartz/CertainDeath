using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.WorldManager
{
    [Table("Worlds")]
    public class GameWorldWrapperWrapper
    {
        [Key]
        public int Id { get; set; }
        public int WorldId { get; set; }
        public virtual GameWorldWrapper Worlds { get; set; }
    }
}
