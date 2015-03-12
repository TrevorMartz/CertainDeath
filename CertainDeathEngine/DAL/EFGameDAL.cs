using CertainDeathEngine.DB;
using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC;
using CertainDeathEngine.Models.World;
using CertainDeathEngine.WorldManager;
using log4net;
using System;
using System.Linq;
using System.Threading;

namespace CertainDeathEngine.DAL
{
    public class EFGameDAL : IGameDAL
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static int _nextWorldId = 1;
        private readonly CDDBModel _cdDbModel;
        private readonly GameWorldGenerator _worldGenerator;
        private readonly WorldManager.WorldManager _worldManager = WorldManager.WorldManager.Instance;

        public EFGameDAL()
        {
            Log.Debug("Constructing EFGameDal");
            _cdDbModel = new CDDBModel();
            _worldGenerator = new GameWorldGenerator();
            SetNextWorldId();
        }

        private static int GetNextId()
        {
            return Interlocked.Increment(ref _nextWorldId);
        }

        public Game CreateGame()
        {
            Log.Debug("Creating new game");
            GameWorld world = CreateWorld();

            // World wasnt stored before, so do it now
            _worldManager.KeepWorld(world);

            // todo move this line too
            SaveWorld(world);
            return GetGameAndStartUpdateThread(world);
        }

        public Game CreateGame(int worldId)
        {
            Log.Debug("Loading existing world: " + worldId);
            GameWorld world = LoadWorld(worldId);
            if (world.HasEnded)
            {
                return null;
                //throw new Exception("The game has already ended");
            }

            return GetGameAndStartUpdateThread(world);
        }

        public bool HasNotEndedVersionOfWorld(int worldId)
        {
            return _cdDbModel.Worlds
                             .Where(x => x.Worlds.HasEnded != true)
                             .OrderByDescending(x => x.Worlds.LastSaveTime)
                             .FirstOrDefault(x => x.WorldId == worldId) != null;
        }

        public bool SaveGame(Game game)
        {
            Log.Debug("Saving game: " + game.World.Id);
            return SaveWorld(game.World);
        }


        private void SetNextWorldId()
        {
            try
            {
                int maxId = 1;
                if (_cdDbModel.Worlds.Any())
                {
                    maxId = _cdDbModel.Worlds.Max(x => x.WorldId);
                }
                _nextWorldId = maxId + 1;
            }
            catch (Exception)
            {
                // TODO do we want to do something better for this exception?
                _nextWorldId = 1;
            }
        }

        private Game GetGameAndStartUpdateThread(GameWorld world)
        {
            Log.Debug("Starting an updater thread for world: " + world.Id);

            if (UpdateManager.Instance.HasWorld(world.Id))
            {
                Log.Debug("Updater is already running.  Ill return that");
                return new Game(world);
            }

            Log.Debug("Really starting the thread now");
            Game g = new Game(world);

            //foreach (var b in world.CurrentTile.Buildings)
            //{
            //    world.AddUpdateMessage(new PlaceBuildingUpdateMessage(b.Id)
            //                           {
            //                               PosX = b.Position.X,
            //                               PosY = b.Position.Y,
            //                               Type = b.Type.ToString()
            //                           });
            //}

            // TODO: move the thread spawn to a better location
            Updater u = new Updater(g);
            Thread updater = new Thread(u.Run);
            Log.Debug("Thread for world " + world.Id + " is thread num " + updater.ManagedThreadId);
            updater.Name = "Updater thread for " + world.Id;
            UpdateManager.Instance.AddGameThread(world.Id, updater);

            return g;
        }

        private bool SaveWorld(GameWorld world)
        {
            Log.Debug("Saving game world: " + world.Id);
            if (_worldManager.HasWorld(world.Id))
            {
                Log.Debug("The WorldManager has the world with id " + world.Id);
                try
                {
                    // todo: update instead of replace the world?
                    GameWorldWrapper wrapper = new GameWorldWrapper()
                                               {
                                                   HasEnded = world.HasEnded,
                                                   World = world,
                                                   LastSaveTime = Environment.TickCount,
                                                   LastUpdateTime = world.TimeLastUpdated

                                               };
                    GameWorldWrapperWrapper wrapperWrapper = new GameWorldWrapperWrapper()
                                                             {
                                                                 Worlds = wrapper,
                                                                 WorldId = world.Id
                                                             };
                    _cdDbModel.Worlds.Add(wrapperWrapper);
                    _cdDbModel.SaveChanges();
                    lock (world)
                    {
                        world.TimeLastSaved = Environment.TickCount;
                    }
                    Log.Debug("Successfully saved the world with id: " + world.Id);
                    return true;
                }
                catch (Exception e)
                {
                    Log.Error("Failed to save the world! " + e.Message);
                    throw new Exception(string.Format("Failed to save the world!!  OOOHHHH NNNOOOOOO {0}", e.Message));
                }
            }
            else
            {
                Log.Error("The WorldManager does NOT have the world with id " + world.Id);
                throw new Exception("The world doesnt exist in the world manager.  you shouldnt have it!!!!!");
            }
        }

        private GameWorld LoadWorld(int worldId)
        {
            Log.Debug("Loading world with Id: " + worldId);
            GameWorld world = null;

            world = _worldManager.GetWorld(worldId);

            if (world != null)
            {
                Log.Debug("World with id " + world.Id + " was already running.  We just returned the instance");
                return world;
            }
            else
            {

                // otherwise we need to get it from the database
                try
                {
                    Log.Debug("Trying to get world with id " + worldId + " from the database");
                    GameWorldWrapperWrapper wrapperwrapper = _cdDbModel.Worlds
                        .Where(x=>x.Worlds.HasEnded != true)
                        .OrderByDescending(x => x.Worlds.LastSaveTime)
                        .FirstOrDefault(x => x.WorldId == worldId);
                    if (wrapperwrapper != null)
                    {
                        Log.Debug("Get world with id " + worldId + " and other of of " + wrapperwrapper.Id + " from the database");
                        world = wrapperwrapper.Worlds.World;
                    }

                    if (world == null)
                    {
                        Log.Error("Can not find the world with the Id " + worldId);
                        throw new Exception("Can not find the world with that Id");
                    }
                }
                catch (Exception e)
                {
                    // TODO do we want to do something better for this exception?
                    Log.Error("We had a problem loading the world: " + e.Message);
                    throw new Exception("We had a problem loading the world", e);
                }

                // World wasnt stored before, so do it now
                Log.Debug("Attempting to store the world with id " + worldId + " in the WorldManager");
                _worldManager.KeepWorld(world);

                // return the world
                return world;
            }
        }

        private GameWorld CreateWorld()
        {
            int worldId = GetNextId();
            Log.Debug("Creating a new world with id " + worldId);
            GameWorld newWorld = _worldGenerator.GenerateWorld(worldId);
            return newWorld;
        }
    }
}
