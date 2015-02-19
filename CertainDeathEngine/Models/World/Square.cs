using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CertainDeathEngine.Models.Resources;
using CertainDeathEngine.Models.World;

namespace CertainDeathEngine.Models
{
    public class Square
    {
        public SquareType Type { get;  set; }
        public Resource Resource { get; set; }
    }
}