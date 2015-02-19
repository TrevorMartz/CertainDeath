using CertainDeathEngine.Models.Resources;
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
        private float _coalRarity = .5f;
        public float CoalRarity
        {
            get { return _coalRarity; }
            set
            {
                _coalRarity = _dirtRarity = validateValue(value);
            }
        }

        private float _woodRarity = .5f;
        public float WoodRarity
        {
            get { return _woodRarity; }
            set
            {
                _woodRarity = _dirtRarity = validateValue(value);
            }
        }

        private float _stoneRarity = .5f;
        public float StoneRarity
        {
            get { return _stoneRarity; }
            set
            {
                _stoneRarity = _dirtRarity = validateValue(value);
            }
        }

        private float _ironRarity = .5f;
        public float IronRarity
        {
            get { return _ironRarity; }
            set
            {
                _ironRarity = _dirtRarity = validateValue(value);
            }
        }

        private float _cornRarity = .5f;
        public float CornRarity
        {
            get { return _cornRarity; }
            set
            {
                _cornRarity = _dirtRarity = validateValue(value);
            }
        }

        private float _grassRarity = .5f;
        public float GrassRarity
        {
            get { return _grassRarity; }
            set
            {
                _grassRarity = validateValue(value);
            }
        }

        private float _dirtRarity = .5f;
        public float DirtRarity
        {
            get { return _dirtRarity; }
            set
            {
                _dirtRarity = validateValue(value);
            }
        }

        private float _sandRarity = .5f;
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
            if (f < .1)
                f = .1f;
            else if (f > .9)
                f = .9f;

            return f;
        }

        public GameWorld GenerateWorld(int worldId)
        {
            return new GameWorld(GenerateTile(), worldId);
        }

        public Tile GenerateTile()
        {
            Tile newTile = new Tile();

            for (int row = 0; row < Tile.SQUARE_SIZE; row++)
            {
                for (int col = 0; col < Tile.SQUARE_SIZE; col++)
                {
					newTile.Squares[row, col].Type = SquareType.GRASS; //would like to change this to start with the most common tile type based on rarity values.
                }
            }
            AddNewBackground(ref newTile, DirtRarity, SquareType.DIRT);
            AddNewBackground(ref newTile, SandRarity, SquareType.SAND);
            return newTile;
        }

        public void AddNewBackground(ref Tile tile, float rarity, SquareType type)
        {
            //if random double is greater than the rarity of the item, stop spreading. pick x number of random points where x is the rarity * 10, rounded down
            for (int i = 0; i < rarity * 10f; i++)
            {
                int row = RandomGen.Random.Next(Tile.SQUARE_SIZE);
                int col = RandomGen.Random.Next(Tile.SQUARE_SIZE);
                while (RandomGen.Random.NextDouble() > rarity)
                {
					tile.Squares[row, col].Type = type;
                    switch (RandomGen.Random.Next(4))
                    {
                        case 0:
                            row += (row < (Tile.SQUARE_SIZE - 1) ? 1 : -1);
                            break;
                        case 1:
                            row -= (row > 0 ? 1 : -1);
                            break;
                        case 2:
                            col += (col < (Tile.SQUARE_SIZE - 1) ? 1 : -1);
                            break;
                        case 3:
                            col -= (col > 0 ? 1 : -1);
                            break;
                    }
                }
            }
        }

        
    }
}
