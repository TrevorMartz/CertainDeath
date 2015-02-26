using CertainDeathEngine.Models;
using CertainDeathEngine.Models.User;
using CertainDeathEngine.Models.World;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CertainDeathEngine.DAL
{
    public class BasicGameDAL : IGameDAL
    {
        private string FilePath;
        static int nextWorldId = 1;
        private GameWorldGenerator gen;
        WorldManager.WorldManager worldManager = WorldManager.WorldManager.Instance;

        public BasicGameDAL(string path)
        {
            FilePath = String.Format("{0}\\World", path);
            gen = new GameWorldGenerator();
            SetNextWorldId();
        }

        private void SetNextWorldId()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(FilePath);
                FileInfo[] worldFiles = di.GetFiles();

                string maxFile = worldFiles.Max(x => x.Name.Substring(0, x.Name.Length - 6));
                int maxFileNumber = int.Parse(maxFile);
                nextWorldId = maxFileNumber + 1;
            }
            catch (Exception)
            {
                // TODO do we want to do something better for this exception?
            }
        }

        public bool SaveWorld(GameWorld world)
        {
            if (worldManager.HasWorld(world.Id))
            {
                try
                {
                    System.IO.Stream ms = File.OpenWrite(String.Format("{0}\\{1}.world", FilePath, world.Id));
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ms, world);
                    ms.Flush();
                    ms.Close();
                    ms.Dispose();
                    return true;
                }
                catch (Exception)
                {
                    throw new Exception("Failed to save the world!!  OOOHHHH NNNOOOOOO");
                }
            }
            else
            {
                throw new Exception("The world doesnt exist in the world manager.  you shouldnt have it!!!!!");
            }
        }

        public EngineInterface LoadGame(int worldId)
        {
            GameWorld world = LoadWorld(worldId);
            Game g = new Game(world, new Player());

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
                DirectoryInfo di = new DirectoryInfo(FilePath);
                FileInfo[] worldFiles = di.GetFiles();

                if (worldFiles.Where(x => x.Name.Substring(0, x.Name.Length - 6).Equals(worldId.ToString())).Count() != 0)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    FileStream fs = File.Open(String.Format("{0}\\{1}.world", FilePath, worldId), FileMode.Open);

                    object obj = formatter.Deserialize(fs);
                    world = (GameWorld)obj;
                    fs.Flush();
                    fs.Close();
                    fs.Dispose();
                }


                else
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
            worldManager.StoreWorld(world);            

            // return the world
            return world;
        }

        private GameWorld CreateWorld()
        {
            int worldId = nextWorldId++;
            GameWorld newWorld = gen.GenerateWorld(worldId);
            SaveWorld(newWorld);
            return newWorld;
        }
    }
}
