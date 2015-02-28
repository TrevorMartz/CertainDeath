using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.WorldManager
{
    public class GameWorldWrapperWrapper
    {
        [Key]
        public int Id { get; set; }
        public virtual GameWorldWrapper Worlds { get; set; }
    }
}
