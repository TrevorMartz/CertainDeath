using log4net;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CertainDeathEngine.Models.User
{
    [Serializable]
    [Table("Users")]
    public class CertainDeathUser
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [Key]
        public int Id { get; set; }
        [Required]
        public MyAppUser FBUser { get; set; }
        public int WorldId { get; set; }
        public string Username { get; set; }
        public long CreateTime { get; set; }
        public long LastLoginTime { get; set; }
    }
}
