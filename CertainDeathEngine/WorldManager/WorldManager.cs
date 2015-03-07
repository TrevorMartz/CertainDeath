using CertainDeathEngine.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CertainDeathEngine.WorldManager
{
    public sealed class WorldManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static readonly WorldManager _instance = new WorldManager();

        public static WorldManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private static Dictionary<int, GameWorldWrapper> _worlds;

        WorldManager()
        {
            if (_worlds == null)
            {
                _worlds = new Dictionary<int, GameWorldWrapper>();
            }
        }

        public bool HasWorld(int worldId)
        {
            lock (_worlds)
            {
                return (_worlds.ContainsKey(worldId));
            }
        }

        public GameWorld GetWorld(int worldId)
        {
            lock (_worlds)
            {
                if (_worlds.ContainsKey(worldId))
                {
                    return _worlds[worldId].World;
                }
                else
                {
                    return null;
                }
            }
        }

        public void KeepWorld(GameWorld world)
        {
            lock (_worlds)
            {
                if (_worlds.ContainsKey(world.Id))
                {
                    throw new Exception("The world is already loaded");
                }
                else
                {
                    _worlds.Add(world.Id, new GameWorldWrapper() { World = world });
                }
            }
        }

        public IEnumerable<string> GetLoadedWorldIds()
        {
            return _worlds.Keys.Select(x => x.ToString()).ToList();
        }
    }
}
