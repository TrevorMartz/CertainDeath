using CertainDeathEngine.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models
{
    public class Player : GameObject
    {
        private Dictionary<ResourceType, int> Resources { get; set; }

        public Player()
        {
            Resources = new Dictionary<ResourceType, int>();

            //add all the reources to the dictionary.
            foreach(ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                Resources.Add(type, 0);
            }
        }

        public void AddResource(ResourceType type, int count)
        {
            Resources[type] = Add(Resources[type], count);
        }

        public void RemoveResource(ResourceType type, int count)
        {
            if (Resources[type] > count)
            {
                Resources[type] -= count;
            }
            else
            {
                throw new Exception("Not enough resources");
            }
        }

        public int GetResourceCount(ResourceType type)
        {
            return Resources[type];
        }

        //this method is just to cap off resources if they overflow over max int. I doubt this will ever happen, but it is possible.
        private int Add(int a, int b)
        {
            if(a + b < 0)
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
