using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CertainDeathEngine.Models.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CertainDeathEngine.Models.User
{
    [Serializable]
    [Table("Scores")]
    public class Score
    {
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

    }
}
