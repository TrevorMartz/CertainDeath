using CertainDeathEngine.Models.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.NPC.Buildings
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Cost
    {
        [JsonProperty]
        public Dictionary<ResourceType, int> Costs { get; private set; }

        public Cost()
        {
            Costs = new Dictionary<ResourceType, int>();
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                Costs.Add(type, 0);
            }
        }

        public void SetCost(ResourceType type, int cost)
        {
            if(cost > 0)
            {
                Costs[type] = cost;
            }
        }
    }
}
