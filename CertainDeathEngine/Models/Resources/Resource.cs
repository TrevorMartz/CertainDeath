using log4net;
using System;

namespace CertainDeathEngine.Models.Resources
{
    [Serializable]
    public class Resource
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static int MaxResources = 30;
        public int Quantity { get; private set; }
		public ResourceType Type { get; set; }

        public Resource(ResourceType type, int quantity)
        {
            this.Type = type;
            this.Quantity = quantity;
        }

        public int Gather(int toTake = 1)
        {
            if(toTake <= 0)
            {
                return 0;
            }

            if (Quantity - toTake < 0)
            {
                //we are trying to take more than we have, but we don't want to give the user more than there was. So make a copy of how much is left to return.
                int toReturn = Quantity;
                Quantity = 0;
                return toReturn;
            }
            else
            {
                Quantity -= toTake;
                return toTake;
            }
        }
    }
}
