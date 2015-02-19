﻿using CertainDeathEngine.Models;
using CertainDeathEngine.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.DAL
{
    public interface IDAL
    {
        void SaveGame(EngineInterface engine);

        EngineInterface LoadGame(CertainDeathUser user);
    }
}
