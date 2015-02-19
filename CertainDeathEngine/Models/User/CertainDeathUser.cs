using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.User
{
    public class CertainDeathUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int FacebookId { get; set; }
        public string Email { get; set; }
        public int WorldId { get; set; }
    }
}
