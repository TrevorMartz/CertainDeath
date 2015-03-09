using CertainDeathEngine.Factories;
using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC.Buildings;
using CertainDeathEngine.Models.Resources;
using CertainDeathEngine.Models.User;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CertainDeathEngine
{
    public class Game : EngineInterface
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GameWorld World;
        public GameFactory BuildingFactory;
        public MonsterGenerator MonsterGenerator;
        private readonly UpdateManager _updateManager;
        private readonly DateTime _worldCreation;
        private readonly Score _worldScore;

        public Game(GameWorld world)
        {
            Log.Info("Constructing Game for world " + world.Id);

            Init.InitAll();
            World = world;
            _worldCreation = new DateTime();
            _worldScore = new Score { FireLevel = 1 };
            _updateManager = UpdateManager.Instance;
            BuildingFactory = new GameFactory(World);
            MonsterGenerator = new MonsterGenerator(World) { InitialSpawnSize = 150, SpawnSize = 1, Delay = 20000, Rate = 10000 };
            MonsterGenerator.Update(1);
        }

        public string ToJSON()
        {
            string jsonString;
            lock (World)
            {
                jsonString = JsonConvert.SerializeObject(World);

            }
            return jsonString;
        }

        public string SquareClicked(float row, float col)
        {
            lock (World)
            {
                Resource res = World.CurrentTile.Squares[(int)row, (int)col].Resource;
                if (res != null)
                {
                    ResourceType type = res.Type;
                    int gathered = World.CurrentTile.Squares[(int)row, (int)col].GatherResource();
                    World.Player.AddResource(type, gathered);

                    _worldScore.AddResource(type, gathered);
                }
            }
            return ToJSON();
        }

        public string MonsterClicked(int monsterid)
        {
            //if monster died WorldScore.Kills++
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
            return Enum.GetValues(typeof (BuildingType)).Cast<BuildingType>().ToList();
        }

        public Building BuildBuildingAtSquare(int row, int column, BuildingType buildingType)
        {

            Building building;
            lock (World)
            {
                // check if it is a good location
                building = BuildingFactory.BuildBuilding(buildingType, new Point(column, row));
                System.Drawing.Point[] cornersAsSquareGrid = building.CornerApproxSquares();
                for (int row2 = cornersAsSquareGrid[0].Y; row2 <= cornersAsSquareGrid[2].Y; row2++)
                {
                    for (int col2 = cornersAsSquareGrid[0].X; col2 <= cornersAsSquareGrid[1].X; col2++)
                    {
                        if (World.CurrentTile.Squares[row2, col2].Building != null)
                        {
                            return null;
                        }
                    }
                }

                World.CurrentTile.AddObject(building);
            }

            // persist the building

            _worldScore.Buildings++;
            return building;
        }

        //Need to add a method for upgrading Fire Level
        //  WorldScore.FireLevel++;

        public void SaveWorld()
        {

            //if (worldManager.HasWorld(World.Id)) {
            //    worldManager.KeepWorld(World);
            //} else {
            //    throw new Exception("The world manager is missing the world"); :'(
            //}
        }

        public void GameOver()
        {
            SaveScore();
            World.HasEnded = true;
            _updateManager.RemoveGameThread(World.Id);
        }

        public void SaveScore()
        {
            _worldScore.Survived = new DateTime() - _worldCreation;
        }
    }
}
