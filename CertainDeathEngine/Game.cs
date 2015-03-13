using CertainDeathEngine.DAL;
using CertainDeathEngine.Factories;
using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC;
using CertainDeathEngine.Models.NPC.Buildings;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace CertainDeathEngine
{
    public class Game
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GameWorld World;
        public GameFactory BuildingFactory;
        public MonsterGenerator MonsterGenerator;
        private readonly UpdateManager _updateManager;
        private readonly long _worldCreation;
        private readonly IStatisticsDAL _statisticsDal = new EFStatisticsDAL();

        public Game(GameWorld world)
        {
            Log.Debug("Constructing Game for world " + world.Id);

            Init.InitAll();
            World = world;
            _worldCreation = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            _updateManager = UpdateManager.Instance;
            BuildingFactory = new GameFactory(World);
            MonsterGenerator = new MonsterGenerator(World) { InitialSpawnSize = 5, SpawnSize = 1, Delay = 20000, Rate = 10000 };
            MonsterGenerator.Update(1);
        }

        public Building BuildBuildingAtSquare(int row, int column, BuildingType buildingType)
        {

            Building building;
            lock (World)
            {
                building = BuildingFactory.BuildBuilding(buildingType, new Point(column, row));
                //check to see if you can afford
                foreach(var v in building.Cost.Costs)
                {
                    if(World.Player.GetResourceCount(v.Key) < v.Value)
                    {
                        return null;
                    }
                }
                
                // check if it is a good location
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
                this.World.AddUpdateMessage(new PlaceBuildingUpdateMessage(building.Id)
                {
                    PosX = building.Position.X,
                    PosY = building.Position.Y,
                    Type = building.Type.ToString()
                });
                World.CurrentTile.AddObject(building);

                Cost buildingCost = building.Cost;
                foreach (var c in buildingCost.Costs)
                {
                    this.World.AddUpdateMessage(new RemoveResourceFromPlayerUpdateMessage(this.World.Player.Id)
                    {
                        Amount = c.Value,
                        ResourceType = c.Key.ToString()
                    });
                    this.World.Player.RemoveResource(c.Key, c.Value);
                }
            }

            // persist the building

            World.Score.Buildings++;
            return building;
        }

        public void GameOver()
        {
            SaveScore();
            World.HasEnded = true;
            _updateManager.RemoveGameThread(World.Id);
        }

        public IEnumerable<BuildingType> GetBuildableBuildingsList()
        {
            return Enum.GetValues(typeof(BuildingType)).Cast<BuildingType>().ToList();
        }

        private string SendBuildigUpdatesBecauseWeChangedSomething()
        {
            //foreach (var b in World.CurrentTile.Buildings)
            //{
            //    World.AddUpdateMessage(new PlaceBuildingUpdateMessage(b.Id)
            //    {
            //        PosX = b.Position.X,
            //        PosY = b.Position.Y,
            //        Type = b.Type.ToString()
            //    });
            //}
            return ToJSON();
        }

        /* Changed the current tile to be the tile directly above the current current tile.
		 * If there is no above tile, does nothing.
		 * Returns ToJSON
		 */
        public string MoveUp()
        {
            if (World.CurrentTile.HasAbove)
            {
                World.CurrentTile = World.CurrentTile.Above;
            }
            return SendBuildigUpdatesBecauseWeChangedSomething();
        }

        /* Changed the current tile to be the tile directly below the current current tile.
		 * If there is no below tile, does nothing.
		 * Returns ToJSON
		 */
        public string MoveDown()
        {
            if (World.CurrentTile.HasBelow)
            {
                World.CurrentTile = World.CurrentTile.Below;
            }
            return SendBuildigUpdatesBecauseWeChangedSomething();
        }

        /* Changed the current tile to be the tile directly to the left of the current current tile.
         * If there is no left tile, does nothing.
         * Returns ToJSON
         */
        public string MoveLeft()
        {
            if (World.CurrentTile.HasLeft)
            {
                World.CurrentTile = World.CurrentTile.Left;
            }
            return SendBuildigUpdatesBecauseWeChangedSomething();
        }

        /* Changed the current tile to be the tile directly to the right of the current current tile.
         * If there is no right tile, does nothing.
         * Returns ToJSON
         */
        public string MoveRight()
        {
            if (World.CurrentTile.HasRight)
            {
                World.CurrentTile = World.CurrentTile.Right;
            }
            return SendBuildigUpdatesBecauseWeChangedSomething();
        }

        public void SaveScore()
        {
            World.Score.Survived = (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - _worldCreation;
            _statisticsDal.SaveScore(World.Score);
        }

        /* Notify the game engine that a user has clicked on a sqaure. If there is a resource on that 
         * square the resource will be added to the user's inventory. Otherwise nothing will happen.
         * 
         * Returns a JSON string representing changes. 
         * JSON described in IncrementTimeAndReturnDelta method
         */
        public void SquareClicked(RowColumnPair click)
        {
            World.AddClick(click);
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
    }
}
