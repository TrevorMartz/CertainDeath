using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CertainDeathEngine.Models
{
    [Serializable]
    public abstract class GameObject
    {

        public Point Position { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public GameObject()
        {
        }

        public GameObject(Point pos)
        {
            this.Position = pos;
        }
    }
}
