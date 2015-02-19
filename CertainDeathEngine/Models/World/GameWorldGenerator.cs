﻿using CertainDeathEngine.Models.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.Models.World
{
    public class GameWorldGenerator
    {
        #region RarityValues
        //A higher rarity value makes the item rarer. They all default to .5
        private static float defaultValue = .3f;
        private float _coalRarity = defaultValue;
        public float CoalRarity
        {
            get { return _coalRarity; }
            set
            {
                _coalRarity = validateValue(value);
            }
        }

        private float _woodRarity = defaultValue;
        public float WoodRarity
        {
            get { return _woodRarity; }
            set
            {
                _woodRarity = validateValue(value);
            }
        }

        private float _stoneRarity = defaultValue;
        public float StoneRarity
        {
            get { return _stoneRarity; }
            set
            {
                _stoneRarity = validateValue(value);
            }
        }

        private float _ironRarity = defaultValue;
        public float IronRarity
        {
            get { return _ironRarity; }
            set
            {
                _ironRarity = validateValue(value);
            }
        }

        private float _cornRarity = defaultValue;
        public float CornRarity
        {
            get { return _cornRarity; }
            set
            {
                _cornRarity = validateValue(value);
            }
        }

        private float _grassRarity = defaultValue;
        public float GrassRarity
        {
            get { return _grassRarity; }
            set
            {
                _grassRarity = validateValue(value);
            }
        }

        private float _dirtRarity = defaultValue;
        public float DirtRarity
        {
            get { return _dirtRarity; }
            set
            {
                _dirtRarity = validateValue(value);
            }
        }

        private float _sandRarity = defaultValue;
        public float SandRarity
        {
            get { return _sandRarity; }
            set
            {
                _sandRarity = validateValue(value);
            }
        }

        #endregion

        private float validateValue(float f)
        {
            if (f < .1f)
                f = .1f;
            else if (f > .9f)
                f = .9f;

            return f;
        }

        public GameWorld GenerateWorld(int worldSize = 3)
        {            
            Tile[,] baseWorld = new Tile[worldSize, worldSize];
            for (int row = 0; row < worldSize; row++)
            {
                for (int col = 0; col < worldSize; col++)
                {
                    baseWorld[row, col] = GenerateTile();
                }
            }
            for (int row = 0; row < worldSize; row++)
            {
                for (int col = 0; col < worldSize; col++)
                {
                    if(row > 0)
                    {
                        baseWorld[row, col].Above = baseWorld[row - 1, col];
                    }

                    if(row < worldSize - 1)
                    {
                        baseWorld[row, col].Below = baseWorld[row + 1, col];
                    }

                    if (col > 0)
                    {
                        baseWorld[row, col].Left = baseWorld[row, col - 1];
                    }

                    if (col < worldSize - 1)
                    {
                        baseWorld[row, col].Below = baseWorld[row, col + 1];
                    }
                }
            }
            int middle = worldSize / 2;
            //Trace.WriteLine("Middle: " + middle);
            GameWorld newWorld = new GameWorld(baseWorld[middle, middle]);

            //for(int row = 0; row < worldSize; row++)
            //{
            //    Tile[] tiles = new Tile[worldSize];
            //    for (int col = 0; col < worldSize; col++ )
            //    {
            //        tiles[col] = baseWorld[row, col];
            //    }
            //    PrintTilesSideBySide(tiles);
            //    Trace.WriteLine("");
            //}
            return newWorld;
        }

        public Tile GenerateTile()
        {
            Tile newTile = new Tile();

            for (int row = 0; row < Tile.SQUARE_SIZE; row++)
            {
                for (int col = 0; col < Tile.SQUARE_SIZE; col++)
                {
                    newTile.Squares[row, col].Type = SquareType.GRASS; //would like to change this to start with the most common tile type based on rarity values... big task
                }
            }
            AddNewBackground(ref newTile, DirtRarity, SquareType.DIRT);
            AddNewBackground(ref newTile, SandRarity, SquareType.SAND);
            return newTile;
        }

        public void AddNewBackground(ref Tile tile, float rarity, SquareType type)
        {
            //if random double is greater than the rarity of the item, stop spreading. pick x number of random points where x is the rarity * 10, rounded down
            for (float i = 1; i > rarity; i -= .1f)
            {
                int row = RandomGen.Random.Next(Tile.SQUARE_SIZE);
                int col = RandomGen.Random.Next(Tile.SQUARE_SIZE);
                while (RandomGen.Random.NextDouble() > rarity)
                {
                    for (int j = 0; j < RandomGen.Random.Next(10) + 5; j++)
                    {
                        tile.Squares[row, col].Type = type;
                        switch (RandomGen.Random.Next(4))
                        {
                            case 0://east
                                row += (row < (Tile.SQUARE_SIZE - 1) ? 1 : -1);
                                break;
                            case 1://west
                                row -= (row > 0 ? 1 : -1);
                                break;
                            case 2://sout
                                col += (col < (Tile.SQUARE_SIZE - 1) ? 1 : -1);
                                break;
                            case 3://north
                                col -= (col > 0 ? 1 : -1);
                                break;
                        }
                    }
                }
            }
        }

        public static void PrintTilesSideBySide(Tile[] tiles)
        {
            for (int row = 0; row < Tile.SQUARE_SIZE; row++)
            {
                foreach (Tile t in tiles)
                {
                    for (int col = 0; col < Tile.SQUARE_SIZE; col++)
                    {
                        Trace.Write((int)t.Squares[row, col].Type);
                    }
                    Trace.Write(" ");
                }
                Trace.WriteLine("");
            }
        }
    }
}
