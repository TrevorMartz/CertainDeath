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

        public RowColumnPair(System.Drawing.Point p)
        {
            Row = p.Y;
            Column = p.X;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != this.GetType()) { return false; }
            return Equals((RowColumnPair) obj);
        }
        protected bool Equals(RowColumnPair other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public override int GetHashCode()
        {
            unchecked { return (Row * 397) + Column * 17; }
        }
    }
}
