using CertainDeathEngine.Models;
using CertainDeathEngine.Models.User;
using CertainDeathEngine.Models.World;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.DAL
{
    public class BasicGameDAL : IGameDAL
    {
        private string FilePath;
        static int nextWorldId = 1;
        private GameWorldGenerator gen;

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
                int poop = 5;
            }
        }

        public void SaveWorld(GameWorld world)
        {
            StreamWriter fs = new StreamWriter(String.Format("{0}\\{1}.world", FilePath, world.Id), true);

            string worldJson = JsonConvert.SerializeObject(world);  
            fs.WriteLine(worldJson);
            fs.Flush();
            fs.Close();
            fs.Dispose();
        }

        public EngineInterface LoadGame(int worldId)
        {
            GameWorld world = LoadWorld(worldId);
            Game g = new Game(world, new Player());
            return g;
        }

        public GameWorld LoadWorld(int worldId)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(FilePath);
                FileInfo[] worldFiles = di.GetFiles();

                if (worldFiles.Where(x => x.Name.Substring(0, x.Name.Length - 6).Equals(worldId.ToString())).Count() != 0)
                {
                    StreamReader fs = new StreamReader(String.Format("{0}\\{1}.world", FilePath, worldId));
                    //FileStream fs = File.Open(String.Format("{0}\\{1}.world", FilePath, worldId), FileMode.Open);
                    string worldJson = fs.ReadToEnd();

                    var world = JsonConvert.DeserializeObject<GameWorld>(worldJson);

                    //object obj = formatter.Deserialize(fs);
                    //ProductList products = (ProductList)obj;
                    fs.Close();
                    fs.Dispose();
                    return world;

                    // return the world
                }
                else
                {
                    // there is not a world with that id
                    return CreateWorld();
                }
            }
            catch (Exception)
            {
                // error...
                return CreateWorld();
            }
        }

        public GameWorld CreateWorld()
        {
            int worldId = nextWorldId++;
            GameWorld newWorld = gen.GenerateWorld(worldId);
            SaveWorld(newWorld);
            return newWorld;
        }
    }
}
