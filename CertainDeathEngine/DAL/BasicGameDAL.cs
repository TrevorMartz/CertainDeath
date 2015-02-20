using CertainDeathEngine.Models;
using CertainDeathEngine.Models.User;
using CertainDeathEngine.Models.World;
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

        public BasicGameDAL(string path)
        {
            FilePath = path;
        }

        public void SaveWorld(GameWorld world)
        {
            FileStream fs = File.OpenWrite(String.Format("{0}\\World\\{1}.world", FilePath, world.Id));
            //fs.Write();

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
                FileStream fs = File.Open(String.Format("{0}\\World\\{1}.world", FilePath, worldId), FileMode.Open);

                //object obj = formatter.Deserialize(fs);
                //ProductList products = (ProductList)obj;
                //fs.Flush();
                //fs.Close();
                //fs.Dispose();
                return new GameWorldGenerator().GenerateWorld(worldId);
            }
            catch (Exception e)
            {
                return new GameWorldGenerator().GenerateWorld(worldId);
            }
        }
    }
}
