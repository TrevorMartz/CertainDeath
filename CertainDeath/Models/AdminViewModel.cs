using CertainDeathEngine.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CertainDeath.Models
{
    public class AdminViewModel
    {
        public IEnumerable<string> LoadedWorlds { get; set; }
        public IEnumerable<string> UpdateThreads { get; set; }
        public IEnumerable<CertainDeathUser> Users { get; set; }
    }
}