using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace CertainDeathEngine.Models.User
{
    [Serializable]
    [Table("Users")]
    public class CertainDeathUser
    {
        [Key]
        public int Id { get; set; }
        public MyAppUser FBUser { get; set; }
        public int WorldId { get; set; }
        //public Score Score { get; set; } <--This will probably not be necessary since the world now holds a score which will store a UserID
    }
}
