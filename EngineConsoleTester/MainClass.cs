using CertainDeathEngine;
using CertainDeathEngine.DAL;
using CertainDeathEngine.Models;
using CertainDeathEngine.Models.World;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// This project will be used to provide console output for the CertainDeathEngine
// for testing and debuging
namespace EngineConsoleTester
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            Init.InitAll();
            //ShayneTests();
            //BlakeIsSOOOOOOUgly();
            TrevorTests();

            Console.ReadLine();
        }

        public static void TrevorTests()
        {
            Player p = new Player();
        }

        private static void BlakeIsSOOOOOOUgly()
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
            EngineInterface g = new Game(generator.GenerateWorld(7), new Player());
            string json = g.ToJSON();
            Console.WriteLine(json);
        }
    }
}
