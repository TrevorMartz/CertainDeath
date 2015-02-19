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
			Init.InitAll();
			GameWorldGenerator generator = new GameWorldGenerator();
			generator.GenerateTile().PrintTile();
		}
	}
}
