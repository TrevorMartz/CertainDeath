﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.NPC
{
    [Serializable]
    public class Building : Killable
    {
        public float HarvestRate { get; set; }
    }
}
