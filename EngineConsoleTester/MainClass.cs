﻿using CertainDeathEngine;
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
            BlakeIsSOOOOOOSexy();
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
            Game g = (Game)GameDAL.LoadGame(2);
            //GameWorld loaded = GameDAL.LoadWorld(2);


        }

        public static void ShayneTests()
        {
            Init.InitAll();
            GameWorldGenerator generator = new GameWorldGenerator();
            EngineInterface g = new Game(generator.GenerateWorld(7));
            string json = g.ToJSON();
            Console.WriteLine(json);
        }
    }
}
