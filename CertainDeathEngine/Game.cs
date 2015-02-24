﻿using CertainDeathEngine.Models;
using CertainDeathEngine.Models.User;
﻿using CertainDeathEngine.Factories;
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
using CertainDeathEngine.Models.Resources;
using System.Diagnostics;

namespace CertainDeathEngine
{
	public class Game : EngineInterface
	{
        Player player;
		public GameWorld World;
        public GameFactory buildingFactory;
		public MonsterGenerator MonsterGenerator;

        public Game(GameWorld world, Player player)
        {
            Init.InitAll();
            World = world;
            buildingFactory = new GameFactory();
			MonsterGenerator = new MonsterGenerator(buildingFactory, World.Tiles) 
				{ InitialSpawnSize = 15, SpawnSize = 1, Delay = 0, Rate = 10000 };
			MonsterGenerator.Update(1);
        }

        public string ToJSON()
		{
			return JsonConvert.SerializeObject(World.CurrentTile);
		}

        public string SquareClicked(float row, float col)
		{
            Resource res = World.CurrentTile.Squares[(int)row, (int)col].Resource;
            if(res != null)
            {
                ResourceType type = res.Type;
                int gathered = World.CurrentTile.Squares[(int)row, (int)col].GatherResource();
                player.AddResource(type, gathered);
                Trace.WriteLine("Resource: " + type + " player count: " + player.GetResourceCount(type));
            }
            return ToJSON();
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

    }
}
