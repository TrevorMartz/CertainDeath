using CertainDeathEngine.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine
{
	class Game : EngineInterface
	{
		GameWorld World;
		public Game()
		{
			Init.InitAll();
			World = new GameWorld();
		}

		string EngineInterface.ToJSON()
		{
			String json = JsonConvert.ToString(World.CurrentTile);
			return json;
		}

		void EngineInterface.MoveUp()
		{
			throw new NotImplementedException();
		}

		void EngineInterface.MoveDown()
		{
			throw new NotImplementedException();
		}

		void EngineInterface.MoveLeft()
		{
			throw new NotImplementedException();
		}

		void EngineInterface.MoveRight()
		{
			throw new NotImplementedException();
		}

		string EngineInterface.SquareClicked(int x, int y)
		{
			throw new NotImplementedException();
		}

		string EngineInterface.MonsterClicked(int monsterid)
		{
			throw new NotImplementedException();
		}

		string EngineInterface.IncrementTimeAndReturnDeta(int millis)
		{
			throw new NotImplementedException();
		}
	}
}
