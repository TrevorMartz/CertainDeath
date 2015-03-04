using CertainDeathEngine.Models;
using log4net;

namespace CertainDeathEngine
{
    public static class Init
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public static bool Initted { get; private set; }
        public static void InitAll()
        {
			if (!Initted)
			{
				RandomGen.Init();
				Tile.InitSize();
				Initted = true;
			}
        }
    }
}
