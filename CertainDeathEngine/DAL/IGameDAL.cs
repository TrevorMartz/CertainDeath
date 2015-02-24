using CertainDeathEngine.Models;
using CertainDeathEngine.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.DAL
{

    // for blake
    // http://www.reddnet.net/entity-framework-json-column/
    public interface IGameDAL
    {
        void SaveWorld(GameWorld world);

        EngineInterface LoadGame(int worldId);

        GameWorld CreateWorld();

        //GameWorld LoadWorld(int worldId);

        //Tile LoadTile(int tileId);
    }
}
