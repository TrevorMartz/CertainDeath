using System;
using System.Collections.Generic;
using CertainDeathEngine.Models.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using log4net;

namespace CertainDeathEngine.Models.User
{
    [Serializable]
    [Table("Scores")]
    public class Score
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int WorldId { get; set; }
        public DateTime SaveDate { get; set; }
        public int Kills { get; set; }
        public int Buildings { get; set; }
        public Dictionary<ResourceType, int> ResourcesCollected { get; set; }
        public TimeSpan Survived { get; set; }
        public int FireLevel { get; set; }

        public Score()
        {
            ResourcesCollected = new Dictionary<ResourceType, int>();
        }

        public void AddResource(ResourceType type, int count)
        {
            lock (ResourcesCollected)
            {
                if (ResourcesCollected.ContainsKey(type))
                {
                    ResourcesCollected[type] = Add(ResourcesCollected[type], count);
                }
                else
                {
                    ResourcesCollected[type] = count;
                }
            }
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
