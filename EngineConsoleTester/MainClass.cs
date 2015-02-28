using CertainDeathEngine;
using CertainDeathEngine.DAL;
using CertainDeathEngine.Models;
using CertainDeathEngine.Models.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using CertainDeathEngine.Models.NPC;
using System.Threading;
using CertainDeathEngine.Models.User;
using System.Globalization;


// This project will be used to provide console output for the CertainDeathEngine
// for testing and debuging
namespace EngineConsoleTester
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            Init.InitAll();
            ShayneTests();
            //BlakeIsSOOOOOOUgly();
            TrevorTests();

            Console.ReadLine();
        }

        public static void TrevorTests()
        {

            GameWorldGenerator gen = new GameWorldGenerator();
            //gen.GenerateWorld(12);
            Game game = new Game(gen.GenerateWorld(3, 3, false));
            while (true)
            {
                game.World.CurrentTile.PrintTileResources();
                string row = Console.ReadLine();
                string col = Console.ReadLine();
                game.SquareClicked(float.Parse(row, CultureInfo.InvariantCulture), float.Parse(col, CultureInfo.InvariantCulture));
                game.World.CurrentTile.PrintTileResources();
            }
            //Console.ReadLine();
        }

        private static void BlakeIsSOOOOOOUgly()
        {
            IGameDAL GameDAL = new BasicGameDAL("c:\\_\\test\\");
            IUserDAL UserDAL = new EFUserDAL();// BasicUserDAL("c:\\_\\test\\");

            Game g = null;
            g = (Game)GameDAL.LoadGame(10);
            GameDAL.SaveWorld(g.World);
            while (true)
            {

                PrintGame(g);
            }
            //Game g = (Game)GameDAL.LoadGame(2);
            //GameWorld loaded = GameDAL.LoadWorld(2);


        }

        public static void ShayneTests()
        {
			GameWorldGenerator generator = new GameWorldGenerator();
			Game g = new Game(generator.GenerateWorld(3));
            IncrementTime(g);
			string json = g.ToJSON();
			Console.WriteLine(json);
        }

		public static void IncrementTime(Game g)
		{
			while (true)
			{
				g.MonsterGenerator.Update(500);
				PrintGame(g);
				foreach (Tile t in g.World.Tiles)
				{
					IEnumerable<Temporal> timeObjects = new List<Temporal>(t.Objects.OfType<Temporal>());
					foreach (Temporal tim in timeObjects)
						tim.Update(500);
				}
				Console.ReadLine();
			}
		}

		public static void PrintGame(Game g)
		{
			List<System.Drawing.Point> MonsterSquares = new List<System.Drawing.Point>();
			foreach (Monster m in g.World.CurrentTile.Monsters)
			{
				MonsterSquares.Add(m.ApproxSquare());
			}

			for (int y = 0; y < Tile.SQUARE_SIZE; y++)
			{
				for (int x = 0; x < Tile.SQUARE_SIZE; x++)
				{
					System.Drawing.Point squarePoint = new System.Drawing.Point(x, y);
					Square square = g.World.CurrentTile.Squares[y,x];
					if (square.Building != null)
					{
						Console.Write("B");
					}
					else if (MonsterSquares.Contains(squarePoint))
					{
						Console.Write("M");
					}
					//else if (g.World.CurrentTile.Squares[y, x].Resource != null)
					//{
					//	Console.Write("R");
					//}
					else
					{
						Console.Write(".");
					}
				}
				Console.WriteLine();
			}
		}
    }
}
