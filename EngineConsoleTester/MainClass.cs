using CertainDeathEngine;
using CertainDeathEngine.Models.World;
using System;
using System.Collections.Generic;
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
			ShayneTests();
			Init.InitAll();
			GameWorldGenerator generator = new GameWorldGenerator();
			generator.GenerateTile().PrintTile();
		}

		public static void ShayneTests()
		{
            
			GameWorldGenerator generator = new GameWorldGenerator();
            EngineInterface g = new Game(generator.GenerateWorld(7));
			string json = g.ToJSON();
			Console.WriteLine(json);
		}
	}
}
