using CertainDeathEngine.DB;
using CertainDeathEngine.Models;
using CertainDeathEngine.Models.World;
using CertainDeathEngine.WorldManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CertainDeathEngine.DAL
{
    public class EFGameDAL : IGameDAL
    {
        static int nextWorldId = 1;
        private CDDBModel cdDBModel;
        private GameWorldGenerator gen;
        WorldManager.WorldManager worldManager = WorldManager.WorldManager.Instance;

        public EFGameDAL()
        {
            cdDBModel = new CDDBModel();
            gen = new GameWorldGenerator();
            SetNextWorldId();
        }

        private void SetNextWorldId()
        {
            try
            {
                // TODO: we need to figure out the id part of the world storage
                int maxId = cdDBModel.Worlds.Max(x => x.WorldId);
                nextWorldId = maxId + 1;
            }
            catch (Exception)
            {
                // TODO do we want to do something better for this exception?
                nextWorldId = 1;
            }
        }

        public bool SaveWorld(GameWorld world)
        {
            //throw new NotImplementedException();
            if (worldManager.HasWorld(world.Id))
            {
                try
                {
                    cdDBModel.Worlds.Add(new GameWorldWrapperWrapper() { Worlds = new GameWorldWrapper() { World = world }, WorldId = world.Id });
                    cdDBModel.SaveChanges();
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

        public EngineInterface LoadGame(int worldId)
        {
            //throw new NotImplementedException();
            GameWorld world = LoadWorld(worldId);
            if (world.HasEnded)
            {
                throw new Exception("The game has already ended");
            }
            SaveWorld(world);
            Game g = new Game(world);

            // TODO: move the thread spawn to a better location
            Updater u = new Updater(g);
            Thread updater = new Thread(u.Run);
            updater.Name = "Updater thread";
            UpdateManager.Instance.AddGameThread(world.Id, updater);

            return g;
        }

        private GameWorld LoadWorld(int worldId)
        {
            GameWorld world = null;

            world = worldManager.GetWorld(worldId);

            if (world != null)
            {
                return world;
            }

            try
            {
                GameWorldWrapperWrapper wrapperwrapper = cdDBModel.Worlds.Where(x => x.WorldId == worldId).FirstOrDefault();
                if (wrapperwrapper != null)
                {
                    world = wrapperwrapper.Worlds.World;
                }
                //world = cdDBModel.Worlds.Where(x => x.Worlds.World.Id == worldId).First().Worlds.World;

                if (world == null)
                {
                    // there is not a world with that id
                    world = CreateWorld();
                }
            }
            catch (Exception)
            {
                // TODO do we want to do something better for this exception?
                world = CreateWorld();
            }

            // World wasnt stored before, so do it now
            worldManager.KeepWorld(world);

            // return the world
            return world;
        }

        private GameWorld CreateWorld()
        {
            int worldId = nextWorldId++;
            GameWorld newWorld = gen.GenerateWorld(worldId);
            return newWorld;
        }
    }
}
