using CertainDeathEngine.Models;
using CertainDeathEngine.Models.World;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace CertainDeathEngine.DAL
{
    public class BasicGameDAL : IGameDAL
    {
        private static int _nextWorldId = 1;
        private readonly string _filePath;
        private readonly GameWorldGenerator _worldGenerator;
        private readonly WorldManager.WorldManager _worldManager = WorldManager.WorldManager.Instance;

        public BasicGameDAL(string path)
        {
            _filePath = String.Format("{0}\\World", path);
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
                DirectoryInfo di = new DirectoryInfo(_filePath);
                FileInfo[] worldFiles = di.GetFiles();

                string maxFile = worldFiles.Max(x => x.Name.Substring(0, x.Name.Length - 6));
                int maxFileNumber = int.Parse(maxFile);
                _nextWorldId = maxFileNumber + 1;
            }
            catch (Exception)
            {
                // TODO do we want to do something better for this exception?
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
                    System.IO.Stream ms = File.OpenWrite(String.Format("{0}\\{1}.world", _filePath, world.Id));
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ms, world);
                    ms.Flush();
                    ms.Close();
                    ms.Dispose();
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
                return world;
            }

            try
            {
                DirectoryInfo di = new DirectoryInfo(_filePath);
                FileInfo[] worldFiles = di.GetFiles();

                if (worldFiles.Count(x => x.Name.Substring(0, x.Name.Length - 6).Equals(worldId.ToString())) != 0)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    FileStream fs = File.Open(String.Format("{0}\\{1}.world", _filePath, worldId), FileMode.Open);

                    object obj = formatter.Deserialize(fs);
                    world = (GameWorld)obj;
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                }

                if (world == null)
                {
                    // there is not a world with that id
                    throw new Exception("Can not find the world with that Id");
                }
            }
            catch (Exception)
            {
                // TODO do we want to do something better for this exception?
                throw new Exception("We had a problem loading the world");
            }

            // World wasnt stored before, so do it now
            _worldManager.KeepWorld(world);

            // return the world
            return world;
        }

        private GameWorld CreateWorld()
        {
            int worldId = GetNextId();
            GameWorld newWorld = _worldGenerator.GenerateWorld(worldId);
            return newWorld;
        }
    }
}
