using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CertainDeathEngine.Models.Resources;

namespace CertainDeathEngine.Models
{
    public class Square
    {
        public const string DEFAULT_SQUARE = "\\Content\\default_square.png";
        public const string GRASS = "\\Content\\Grass.png";
        public const string DIRT = "\\Content\\Dirt.png";
        public const string SAND = "\\Content\\Sand.png";

        public Image BackgroundImage { get; private set; }
        public Resource Resource { get; set; }

        public Square()
        {
            SetBackground(Square.DEFAULT_SQUARE);
        }

        public void SetBackground(string imagePath)//image is ment to be one of the const strings in the class.
        {
            this.BackgroundImage = Image.FromFile(imagePath);
        }
    }
}