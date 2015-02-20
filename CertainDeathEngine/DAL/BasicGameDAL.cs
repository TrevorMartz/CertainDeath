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

        public BasicGameDAL(string path)
        {
            FilePath = String.Format("{0}\\World", path);
        }

        public void SaveWorld(GameWorld world)
        {
            StreamWriter fs = new StreamWriter(String.Format("{0}\\{1}.world", FilePath, world.Id), true);

            string worldJson = JsonConvert.SerializeObject(world.CurrentTile);  
            fs.WriteLine(worldJson);
            fs.Flush();
            fs.Close();
            fs.Dispose();
        }

        public EngineInterface LoadGame(int worldId)
        {
            GameWorld world = LoadWorld(worldId);
            Game g = new Game(world);
            return g;
        }

        public GameWorld LoadWorld(int worldId)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(FilePath);
                FileInfo[] worldFiles = di.GetFiles();

                if (worldFiles.Where(x => x.Name.Equals(worldId.ToString())).Count() != 0)
                {
                    StreamReader fs = new StreamReader(String.Format("{0}\\{1}.world", FilePath, worldId));
                    //FileStream fs = File.Open(String.Format("{0}\\{1}.world", FilePath, worldId), FileMode.Open);
                    string worldJson = fs.ReadToEnd();

                    GameWorld world = (GameWorld)JsonConvert.DeserializeObject(worldJson);

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
            catch (Exception e)
            {
                // error...
                return CreateWorld();
            }
        }

        public GameWorld CreateWorld()
        {
            int worldId = nextWorldId++;
            return new GameWorldGenerator().GenerateWorld(worldId);
        }
    }
}
