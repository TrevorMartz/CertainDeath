using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertainDeathEngine.Models
{
    public class Tile
    {
        public Square[][] Squares { get; set; }
        public Tile Left { get; set; }
        public Tile Above { get; set; }
        public Tile Right { get; set; }
        public Tile Below { get; set; }
    }
}
