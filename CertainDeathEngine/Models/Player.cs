using CertainDeathEngine.Models.Resources;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CertainDeathEngine.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Player : GameObject
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [JsonProperty]
        public Dictionary<ResourceType, int> Resources { get; private set; }

        public Player()
        {
            Resources = new Dictionary<ResourceType, int>();

            //add all the reources to the dictionary. Does not need lock because it is a constructor. Multiple threads will never be in the same constructor.
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                Resources.Add(type, 100);
            }
        }

        public void AddResource(ResourceType type, int count)
        {
            lock(Resources)
            {
                Resources[type] = Add(Resources[type], count);
            }
        }

        public void RemoveResource(ResourceType type, int count)
        {
            lock (Resources)
            {
                if (Resources[type] > count)
                {
                    Resources[type] -= count;
                }
            }
        }

        public int GetResourceCount(ResourceType type)
        {
            int count;
            lock(Resources)
            {
                count = Resources[type];
            }
            return count;
        }

        //this method is just to cap off resources if they overflow over max int. I doubt this will ever happen, but it is possible.
        private int Add(int a, int b)
        {
            if (a + b < 0)
            {
                return int.MaxValue;
            }
            else
            {
                return a + b;
            }
        }
    }
}
