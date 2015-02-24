using CertainDeathEngine.Models;
using CertainDeathEngine.Models.User;
using CertainDeathEngine.Models.World;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CertainDeathEngine
{
	public class Game : EngineInterface
	{
		GameWorld World;
        Player player;

        public Game(GameWorld world, Player player)
        {
            Init.InitAll();
            World = world;
        }

		string EngineInterface.ToJSON()
		{
			return JsonConvert.SerializeObject(World.CurrentTile);
		}

		string EngineInterface.SquareClicked(int x, int y)
		{
			throw new NotImplementedException();
		}

		string EngineInterface.MonsterClicked(int monsterid)
		{
			throw new NotImplementedException();
		}

		string EngineInterface.IncrementTimeAndReturnDelta(int millis)
		{
			throw new NotImplementedException();
		}

		string EngineInterface.MoveUp()
		{
			if (World.CurrentTile.HasAbove)
			{
				World.CurrentTile = World.CurrentTile.Above;
			}
			return ((EngineInterface)this).ToJSON();
		}

		string EngineInterface.MoveDown()
		{
			if (World.CurrentTile.HasBelow)
			{
				World.CurrentTile = World.CurrentTile.Below;
			}
			return ((EngineInterface)this).ToJSON();
		}

		string EngineInterface.MoveLeft()
		{
			if (World.CurrentTile.HasLeft)
			{
				World.CurrentTile = World.CurrentTile.Left;
			}
			return ((EngineInterface)this).ToJSON();
		}

		string EngineInterface.MoveRight()
		{
			if (World.CurrentTile.HasRight)
			{
				World.CurrentTile = World.CurrentTile.Right;
			}
			return ((EngineInterface)this).ToJSON();
		}
	}
}
