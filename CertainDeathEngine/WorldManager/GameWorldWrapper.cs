using CertainDeathEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.WorldManager
{
    public class GameWorldWrapper
    {
        public GameWorld World { get; set; }
        public long LastUpdateTime { get; set; }
        public long LastSaveTime { get; set; }
    }
}
