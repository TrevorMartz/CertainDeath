using CertainDeathEngine.Models;
using CertainDeathEngine.Models.User;
using CertainDeathEngine.Models.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.DAL
{
    public class FileDAL : IDAL
    {
        public void SaveGame(EngineInterface engine)
        {
            throw new NotImplementedException();
        }

        public EngineInterface LoadGame(CertainDeathUser user)
        {
            int worldId = user.WorldId;
            GameWorld world = new GameWorldGenerator().GenerateWorld(worldId);
            Game g = new Game(world);
            return g;
        }
    }
}
