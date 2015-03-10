using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CertainDeath.Models
{
    public class StartScreenViewModel
    {
        public bool HowToPlay { get; set; }
        public bool LoadGame { get; set; }
        public bool NewGame { get; set; }
        public int UserId { get; set; }

        public StartScreenViewModel(int id)
        {
            HowToPlay = true;
            LoadGame = false;
            NewGame = true;
            UserId = id;
        }
    }
}