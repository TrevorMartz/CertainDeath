using CertainDeathEngine.Models;
using CertainDeathEngine.Models.User;
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
using CertainDeathEngine.Factories;
using CertainDeathEngine.Models.NPC.Buildings;
using System.Windows;
using CertainDeathEngine.DAL;

namespace CertainDeathEngine
{
    public class Game : EngineInterface
    {
        WorldManager.WorldManager worldManager;
        UpdateManager updateManager;
        public GameWorld World;
        public GameFactory buildingFactory;
        public MonsterGenerator MonsterGenerator;
        private IStatisticsDAL statisticsDAL;

        public Game(GameWorld world)
        {
            worldManager = WorldManager.WorldManager.Instance;
            updateManager = UpdateManager.Instance;
            Init.InitAll();
            World = world;
            buildingFactory = new GameFactory(World);
            MonsterGenerator = new MonsterGenerator(World) { InitialSpawnSize = 15, SpawnSize = 1, Delay = 0, Rate = 10000 };
            MonsterGenerator.Update(1);
            statisticsDAL = new EFStatisticsDAL();
        }

        public string ToJSON()
        {
            string jsonString;
            lock (World.CurrentTile)
            {
                jsonString = JsonConvert.SerializeObject(World.CurrentTile);

            }
            return jsonString;
        }

        public string SquareClicked(float row, float col)
        {
            Resource res;
            lock (World.CurrentTile)
            {
                res = World.CurrentTile.Squares[(int)row, (int)col].Resource;
                if (res != null)
                {
                    ResourceType type = res.Type;
                    int gathered = World.CurrentTile.Squares[(int)row, (int)col].GatherResource();
                    World.Player.AddResource(type, gathered);
                    //Trace.WriteLine("Resource: " + type + " player count: " + Player.GetResourceCount(type));
                }
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


        public IEnumerable<BuildingType> GetBuildableBuildingsList()
        {
            List<BuildingType> list = new List<BuildingType>();
            foreach (BuildingType t in Enum.GetValues(typeof(BuildingType)))
            {
                list.Add(t);
            }
            return list;
        }

        public Building BuildBuildingAtSquare(int row, int column, BuildingType buildingType)
        {
            lock (World.CurrentTile)
            {
                Building buildingInstance = buildingFactory.BuildBuilding(buildingType, new Point((double)column, (double)row));
            }
            // check if it is a good location

            // persist the building

            return null;
        }

        public void SaveWorld()
        {

            if (worldManager.HasWorld(World.Id)) {
                worldManager.StoreWorld(World);
            } else {
                throw new Exception("The world manager is missing the world");
            }
        }

        public void GameOver()
        {
            SaveScore();
            World.HasEnded = true;
            updateManager.RemoveGameThread(World.Id);
        }

        public void SaveScore()
        {

        }
    }
}
