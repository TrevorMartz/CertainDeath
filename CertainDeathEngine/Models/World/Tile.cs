using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Windows;
using CertainDeathEngine.Models.NPC;
using CertainDeathEngine.Models.NPC.Buildings;
using log4net;

namespace CertainDeathEngine.Models
{
    [Serializable]
    [JsonObject(MemberSerialization.OptIn)]
    public class Tile
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static int SQUARES_PER_ROW_AND_COLUMN = 20;
        public static int TOTAL_PIXELS = SQUARES_PER_ROW_AND_COLUMN * Square.PIXEL_SIZE;
        public static int SQUARES = SQUARES_PER_ROW_AND_COLUMN * SQUARES_PER_ROW_AND_COLUMN;

        public GameWorld World { get; set; }


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
        public List<GameObject> Objects { get; private set; }

        [JsonProperty]
        // Do not add or remove to this directly. Use AddObject and RemoveObject methods below.
        public List<Monster> Monsters { get; private set; }

        [JsonProperty]
        // Do not add or remove to this directly. Use AddObject and RemoveObject methods below.
        public List<Building> Buildings { get; private set; }

        public Point Position { get; private set; }

        public Tile(int x, int y, GameWorld world)
        {
            InitSquares();
            Position = new Point(x, y);
            this.Objects = new List<GameObject>();
            this.Monsters = new List<Monster>();
            this.Buildings = new List<Building>();
            this.World = world;
        }

        public void InitSquares()
        {
            //lock (World)
            //{
                this.Squares = new Square[SQUARES_PER_ROW_AND_COLUMN, SQUARES_PER_ROW_AND_COLUMN];
                for (int row = 0; row < SQUARES_PER_ROW_AND_COLUMN; row++)
                {
                    for (int col = 0; col < SQUARES_PER_ROW_AND_COLUMN; col++)
                    {
                        Squares[row, col] = new Square();
                    }
                }
            //}
        }

        public Building GetBuildingAtPoint(Point p)
        {
            Building building = null;
            lock (World)
            {
                foreach (Building b in Buildings)
                {
                    if (b.ContainsPoint(p))
                    {
                        building = b;
                        break;
                    }
                }
            }
            return building;
        }

        public void AddObject(GameObject obj)
        {
            Objects.Add(obj);
            if (obj is Monster)
            {
                lock (World)
                {
                    Monsters.Add(obj as Monster);
                }
            }
            else if (obj is Building)
            {
                Building building = obj as Building;
                lock (World)
                {
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
        }

        public void RemoveObject(GameObject obj)
        {
            Objects.Remove(obj);
            if (obj is Monster)
            {
                lock (World)
                {
                    Monsters.Remove(obj as Monster);
                }
            }
            else if (obj is Building)
            {
                Building building = obj as Building;
                lock (World)
                {
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
        }

        public static void InitSize()
        {
            try
            {
                string s = ConfigurationManager.AppSettings["TileSquareSize"];
                SQUARES_PER_ROW_AND_COLUMN = Convert.ToInt32(s);
                if (SQUARES_PER_ROW_AND_COLUMN <= 0)
                {
                    SQUARES_PER_ROW_AND_COLUMN = 20;
                }
            }
            catch (Exception)
            {
                SQUARES_PER_ROW_AND_COLUMN = 20;
            }
            SQUARES = SQUARES_PER_ROW_AND_COLUMN * SQUARES_PER_ROW_AND_COLUMN;
        }

        public void PrintTile()
        {
            lock (World)
            {
                for (int row = 0; row < SQUARES_PER_ROW_AND_COLUMN; row++)
                {
                    for (int col = 0; col < SQUARES_PER_ROW_AND_COLUMN; col++)
                    {
                        Trace.Write((int)Squares[row, col].Type);
                    }
                    Trace.WriteLine("");
                }
            }
        }

        public void PrintTileResources()
        {
            lock (World)
            {
                for (int row = 0; row < SQUARES_PER_ROW_AND_COLUMN; row++)
                {
                    for (int col = 0; col < SQUARES_PER_ROW_AND_COLUMN; col++)
                    {
                        if (Squares[row, col].Resource == null)
                        {
                            Trace.Write("-");
                        }
                        else
                        {
                            //Trace.Write("(" + row + "," + col + ")");
                            Trace.Write("{" + (int)Squares[row, col].Resource.Type + " " + Squares[row, col].Resource.Quantity + "}");
                        }
                    }
                    Trace.WriteLine("");
                }
            }
        }

        public void PrintTileBuildings(bool printCoords = false)
        {
            lock (World)
            {
                for (int row = 0; row < SQUARES_PER_ROW_AND_COLUMN; row++)
                {
                    for (int col = 0; col < SQUARES_PER_ROW_AND_COLUMN; col++)
                    {
                        if (Squares[row, col].Building == null)
                        {
                            Trace.Write("-");
                        }
                        else
                        {
                            if (printCoords)
                            {
                                Trace.Write("(" + row + "," + col + ")");
                            }
                            Trace.Write((int)Squares[row, col].Building.Type);
                        }
                    }
                    Trace.WriteLine("");
                }
            }
        }
    }
}
