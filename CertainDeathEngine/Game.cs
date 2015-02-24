using CertainDeathEngine.Factories;
using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC;
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
		public GameWorld World;
        GameFactory buildingFactory;

        public Game(GameWorld world)
        {
            Init.InitAll();
            World = world;
            buildingFactory = new GameFactory();
        }

        public string ToJSON()
		{
            // lock something...
            string jsonString = JsonConvert.SerializeObject(World.CurrentTile);;
            return jsonString;
		}

        public string SquareClicked(int x, int y)
		{
			throw new NotImplementedException();
		}

        public string MonsterClicked(int monsterid)
		{
			throw new NotImplementedException();
		}

        public string IncrementTimeAndReturnDelta(int millis)
		{
			throw new NotImplementedException();
		}

        public string MoveUp()
		{
			if (World.CurrentTile.HasAbove)
			{
				World.CurrentTile = World.CurrentTile.Above;
			}
			return ToJSON();
		}

        public string MoveDown()
		{
			if (World.CurrentTile.HasBelow)
			{
				World.CurrentTile = World.CurrentTile.Below;
            }
            return ToJSON();
		}

        public string MoveLeft()
		{
			if (World.CurrentTile.HasLeft)
			{
				World.CurrentTile = World.CurrentTile.Left;
            }
            return ToJSON();
		}

        public string MoveRight()
		{
			if (World.CurrentTile.HasRight)
			{
				World.CurrentTile = World.CurrentTile.Right;
            }
            return ToJSON();
		}


        public IEnumerable<string> GetBuildableBuildingsList()
        {
            return new List<string>() { "Wall", "Turret", "Lumber Mill" };
        }

        public Building BuildBuildingAtSquare(int row, int column, string buildingType)
        {
            Building buildingInstance = buildingFactory.BuildBuilding(buildingType);
            // check if it is a good location

            // persist the building

            return buildingInstance;
        }

        public void SaveWorld()
        {
            // TODO: make a save game work

            // Move the world creation stuff into the game class
        }

    }
}
