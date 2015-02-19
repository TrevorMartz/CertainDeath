using CertainDeathEngine.Models;
using CertainDeathEngine.Models.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine
{
    public static class Init
    {
		public static bool Initted { get; private set; }
        public static void InitAll()
        {
			if (!Initted)
			{
				RandomGen.InitUnseeded();
				Tile.InitSize();
				Initted = true;
			}
        }
    }
}
