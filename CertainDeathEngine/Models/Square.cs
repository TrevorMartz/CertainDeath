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
        public static string DEFAULT_SQUARE_IMAGE = "\\Content\\default_square.png";

        public Image BackgroundImage { get; set; }
        public Resource Resource { get; set; }

        public Square()
        {
            this.BackgroundImage = Image.FromFile(DEFAULT_SQUARE_IMAGE);
        }
    }
}
