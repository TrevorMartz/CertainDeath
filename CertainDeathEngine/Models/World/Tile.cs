using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Windows;
using CertainDeathEngine.Models.NPC;
using CertainDeathEngine.Models.NPC.Buildings;

namespace CertainDeathEngine.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Tile
    {

        public static int SQUARE_SIZE = 20;
		public static int TOTAL_PIXELS = SQUARE_SIZE * Square.PIXEL_SIZE;
		public static int SQUARES = SQUARE_SIZE * SQUARE_SIZE;


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

		// Do not add or remove to this directly. Use AddObject and RemoveObject methods below.
		public List<GameObject> Objects { get; set; }

		[JsonProperty]
		// Do not add or remove to this directly. Use AddObject and RemoveObject methods below.
		public List<Monster> Monsters { get; set; }

		[JsonProperty]
		// Do not add or remove to this directly. Use AddObject and RemoveObject methods below.
		public List<Building> Buildings { get; set; }

		public Point Position { get; private set; }

        public Tile(int x, int y)
        {
            SetSquares();
			Position = new Point(x, y);
			this.Objects = new List<GameObject>();
			this.Monsters = new List<Monster>();
			this.Buildings = new List<Building>();
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

		public Building GetBuildingAtPoint(Point p)
		{
			foreach (Building b in Buildings)
			{
				if (b.ContainsPoint(p))
				{
					return b;
				}
			}
			return null;
		}

        public void AddObject(GameObject obj)
        {
            Objects.Add(obj);
			if (obj is Monster)
				Monsters.Add(obj as Monster);
			if (obj is Building)
			{
				Building building = obj as Building;
				Buildings.Add(building);
				System.Drawing.Point[] CornersAsSquareGrid = building.CornerApproxSquares();
				for (int row = CornersAsSquareGrid[0].Y; row <= CornersAsSquareGrid[2].Y; row++)
				{
					for (int col = CornersAsSquareGrid[0].X; col <= CornersAsSquareGrid[1].X; col++)
					{
						Squares[row, col].Building = building;
					}
				}
			}
        }

		public void RemoveObject(GameObject obj)
		{
			Objects.Remove(obj);
			if (obj is Monster)
				Monsters.Remove(obj as Monster);
			if (obj is Building)
			{
				Building building = obj as Building;
				Buildings.Remove(building);
				System.Drawing.Point[] CornersAsSquareGrid = building.CornerApproxSquares();
				for (int row = CornersAsSquareGrid[0].Y; row <= CornersAsSquareGrid[2].Y; row++)
				{
					for (int col = CornersAsSquareGrid[0].X; col <= CornersAsSquareGrid[1].X; col++)
					{
						Squares[row, col].Building = null;
					}
				}
			}
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
            SQUARES = SQUARE_SIZE * SQUARE_SIZE;
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

        public void PrintTileResources()
        {
            for (int row = 0; row < SQUARE_SIZE; row++)
            {
                for (int col = 0; col < SQUARE_SIZE; col++)
                {
                    if (Squares[row, col].Resource == null)
                    {
                        Trace.Write("-");
                    }
                    else
                    {//"(" + row + "," + col + ")" +     print the coords with each item
                        Trace.Write("(" + row + "," + col + ")" + (int)Squares[row, col].Resource.Type);
                    }
                }
                Trace.WriteLine("");
            }
        }
    }
}
