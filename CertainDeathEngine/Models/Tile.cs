using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertainDeathEngine.Models
{
    public class Tile
    {
        public const int SQUARE_SIZE = 20;

        public Square[,] Squares { get; set; }
        public Tile Left { get; set; }
        public Tile Above { get; set; }
        public Tile Right { get; set; }
        public Tile Below { get; set; }
        public List<GameObject> Objects { get; set; }

        public Tile()
        {
            this.Squares = new Square[SQUARE_SIZE, SQUARE_SIZE];
            this.Objects = new List<GameObject>();
        }

        public void AddObject(GameObject obj)
        {
            this.Objects.Add(obj);
        }
    }
}
