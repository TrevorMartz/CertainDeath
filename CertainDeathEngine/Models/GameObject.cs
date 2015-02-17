using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CertainDeathEngine.Models
{
    public abstract class GameObject
    {
        public static string DEFAULT_OBJECT_IMAGE = "\\Content\\default_object.png";

        public Image Image { get; set; }
        public Point Position { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public GameObject()
        {
            this.Image = Image.FromFile(DEFAULT_OBJECT_IMAGE);
        }

        public GameObject(Point pos) : this()
        {
            this.Position = pos;
        }
    }
}
