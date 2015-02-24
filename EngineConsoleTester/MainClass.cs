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
            //BlakeIsSOOOOOOSexy();
            //double total = 0;
            //for(int i = 0; i < 1000; i++)
            //{
            //    double d = RandomGen.Random.NextDouble();
            //    total += d;
            //    Trace.WriteLine(d);
            //}
            //Trace.WriteLine(total / 1000);

            //while (true)
            //{
            //    GameWorldGenerator generator = new GameWorldGenerator();
            //    GameWorld world = generator.GenerateWorld(3);

            //    //    generator.GenerateTile().PrintTile();
            //    Console.ReadLine();
            //}
        }

        private static void BlakeIsSOOOOOOSexy()
        {
            IGameDAL GameDAL = new BasicGameDAL("c:\\_\\test\\");
            IUserDAL UserDAL = new BasicUserDAL("c:\\_\\test\\", GameDAL);

            for (int i = 0; i < 2; i++)
            {
                GameWorld w1 = GameDAL.CreateWorld();
                GameDAL.SaveWorld(w1);
            }

            GameWorld loaded = GameDAL.LoadWorld(2);

            int asdf = 7;

        }

        public static void ShayneTests()
        {
            Init.InitAll();
            GameWorldGenerator generator = new GameWorldGenerator();
            Game g = new Game(generator.GenerateWorld(7));
			Console.ReadLine();
			for (int i = 0; i < 50; i++)
			{
				PrintGame(g);
				foreach (Monster m in g.World.CurrentTile.Objects)
				{
					m.Move(500);
				}
				Thread.Sleep(500);
				Console.WriteLine();
			}
			//string json = g.ToJSON();
			//Console.WriteLine(json);
        }

		public static void PrintGame(Game g)
		{
			//List<Point> MonsterSquares = new List<Point>();
			//foreach (Monster m in g.Monsters)
			//{
			//	MonsterSquares.Add(m.ApproxSquare());
			//}

			for (int y = 0; y < Tile.SQUARE_SIZE; y++)
			{
				for (int x = 0; x < Tile.SQUARE_SIZE; x++)
				{
					Point square = new Point(x, y);
					//if (MonsterSquares.Contains(square))
					//{
					//	Console.Write("M");
					//}
					////else if (g.World.CurrentTile.Squares[y, x].Resource != null)
					////{
					////	Console.Write("R");
					////}
					//else
					//{
					//	Console.Write(".");
					//}
				}
				Console.WriteLine();
			}
		}
    }
}
