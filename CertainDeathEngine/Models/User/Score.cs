using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CertainDeathEngine.Models.Resources;

namespace CertainDeathEngine.Models.User
{
    [Serializable]
    public class Score
    {
        public int Kills { get; set; }
        public int Buildings { get; set; }
        public Dictionary<ResourceType, int> ResourcesCollected { get; set; }
        public long Survived { get; set; }
        public int FireLevel { get; set; }

    }
}
