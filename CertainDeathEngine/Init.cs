using CertainDeathEngine.Models;
using log4net;

namespace CertainDeathEngine
{
    public static class Init
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public static bool HasBeenInitialized { get; private set; }
        public static void InitAll()
        {
            Log.Info("Initializing");
			if (!HasBeenInitialized)
			{
                RandomGen.InitUnseeded();
				Tile.InitSize();
				HasBeenInitialized = true;
			}
        }
    }
}
