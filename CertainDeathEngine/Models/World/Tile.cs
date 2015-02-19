using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CertainDeathEngine.Models
{
    public class Tile
    {
        
        public static int SQUARE_SIZE = 20;

        public Square[,] Squares { get; set; }

        #region TileReferences
        private Tile _left;

        public Tile Left
        {
            get { return _left; }
            set 
            { 
                _left = value;
                value._right = this;
            }
        }

        private Tile _right;

        public Tile Right
        {
            get { return _right; }
            set 
            { 
                _right = value;
                value._left = this;
            }
        }

        private Tile _above;

        public Tile Above
        {
            get { return _above; }
            set 
            { 
                _above = value;
                value._below = this;
            }
        }

        private Tile _below;

        public Tile below
        {
            get { return _below; }
            set 
            {
                _below = value;
                value._above = this;
            }
        }
        #endregion

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

        public static void InitSize()
        {
            try
            {
                string s = ConfigurationManager.AppSettings["TileSquareSize"];
                SQUARE_SIZE = Convert.ToInt32(s);
            }
            catch(Exception)
            {
                SQUARE_SIZE = 20;
            }
        }
    }
}
