using CertainDeathEngine.DB;
using CertainDeathEngine.Models;
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
            _cdDbModel = new CDDBModel();
            _worldGenerator = new GameWorldGenerator();
            SetNextWorldId();
        }

        private static int GetNextId()
        {
            return Interlocked.Increment(ref _nextWorldId);
        }

        public EngineInterface CreateGame()
        {
            GameWorld world = CreateWorld();

            // World wasnt stored before, so do it now
            _worldManager.KeepWorld(world);

            // todo move this line too
            SaveWorld(world);
            return GetGameAndStartUpdateThread(world);
        }

        public EngineInterface CreateGame(int worldId)
        {
            GameWorld world = LoadWorld(worldId);
            if (world.HasEnded)
            {
                throw new Exception("The game has already ended");
            }

            return GetGameAndStartUpdateThread(world);
        }

        public bool SaveGame(Game game)
        {
            return SaveWorld(game.World);
        }
        

        private void SetNextWorldId()
        {
            try
            {
                // TODO: we need to figure out the id part of the world storage
                int maxId = _cdDbModel.Worlds.Max(x => x.WorldId);
                _nextWorldId = maxId + 1;
            }
            catch (Exception)
            {
                // TODO do we want to do something better for this exception?
                _nextWorldId = 1;
            }
        }

        private EngineInterface GetGameAndStartUpdateThread(GameWorld world)
        {
            Game g = new Game(world);


            // TODO: move the thread spawn to a better location
            Updater u = new Updater(g);
            Thread updater = new Thread(u.Run);
            updater.Name = "Updater thread";
            UpdateManager.Instance.AddGameThread(world.Id, updater);

            return g;
        }

        private bool SaveWorld(GameWorld world)
        {
            if (_worldManager.HasWorld(world.Id))
            {
                try
                {
                    _cdDbModel.Worlds.Add(new GameWorldWrapperWrapper() { Worlds = new GameWorldWrapper() { World = world }, WorldId = world.Id });
                    _cdDbModel.SaveChanges();
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("Failed to save the world!!  OOOHHHH NNNOOOOOO {0}", e.Message));
                }
            }
            else
            {
                throw new Exception("The world doesnt exist in the world manager.  you shouldnt have it!!!!!");
            }
        }

        private GameWorld LoadWorld(int worldId)
        {
            GameWorld world = null;

            world = _worldManager.GetWorld(worldId);

            if (world != null)
            {
                // Here the game world is already loaded and running, just return it
                return world;
            }
            else
            {

                // otherwise we need to get it from the database
                try
                {
                    GameWorldWrapperWrapper wrapperwrapper = _cdDbModel.Worlds.FirstOrDefault(x => x.WorldId == worldId);
                    if (wrapperwrapper != null)
                    {
                        world = wrapperwrapper.Worlds.World;
                    }

                    if (world == null)
                    {
                        // there is not a world with that id
                        throw new Exception("Can not find the world with that Id");
                    }
                }
                catch (Exception e)
                {
                    // TODO do we want to do something better for this exception?
                    throw new Exception("We had a problem loading the world", e);
                }

                // World wasnt stored before, so do it now
                _worldManager.KeepWorld(world);

                // return the world
                return world;
            }
        }

        private GameWorld CreateWorld()
        {
            int worldId = GetNextId();
            GameWorld newWorld = _worldGenerator.GenerateWorld(worldId);
            return newWorld;
        }
    }
}
