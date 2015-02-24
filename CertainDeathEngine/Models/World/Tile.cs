using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Windows;

namespace CertainDeathEngine.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Tile
    {

        public static int SQUARE_SIZE = 20;
		public static int TOTAL_PIXELS = SQUARE_SIZE * Square.PIXEL_SIZE;


        [JsonProperty]
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

        public Tile Below
        {
            get { return _below; }
            set
            {
                _below = value;
                value._above = this;
            }
        }

        [JsonProperty]
        public bool HasAbove { get { return Above != null; } }
        [JsonProperty]
        public bool HasBelow { get { return Below != null; } }
        [JsonProperty]
        public bool HasLeft { get { return Left != null; } }
        [JsonProperty]
        public bool HasRight { get { return Right != null; } }
        #endregion

        [JsonProperty]
        public List<GameObject> Objects { get; set; }

		public Point TilePosition { get; private set; }

        public Tile(int x, int y)
        {
            SetSquares();
			TilePosition = new Point(x, y);
            this.Objects = new List<GameObject>();
        }

        public void SetSquares()
        {
            this.Squares = new Square[SQUARE_SIZE, SQUARE_SIZE];
            for (int row = 0; row < SQUARE_SIZE; row++)
            {
                for (int col = 0; col < SQUARE_SIZE; col++)
                {
                    Squares[row, col] = new Square();
                }
            }
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
                if (SQUARE_SIZE <= 0)
                {
                    SQUARE_SIZE = 20;
                }
            }
            catch (Exception)
            {
                SQUARE_SIZE = 20;
            }
        }

        public void PrintTile()
        {
            for (int row = 0; row < SQUARE_SIZE; row++)
            {
                for (int col = 0; col < SQUARE_SIZE; col++)
                {
                    Trace.Write((int)Squares[row, col].Type);
                }
                Trace.WriteLine("");
            }
        }
    }
}
