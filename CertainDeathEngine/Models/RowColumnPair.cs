using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models
{
    public class RowColumnPair
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public RowColumnPair(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
