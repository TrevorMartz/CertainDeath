using CertainDeathEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.DAL
{
    public interface IDAL
    {
        void SaveWorld(GameWorld world);

        GameWorld LoadWorld(int worldId);
    }
}
