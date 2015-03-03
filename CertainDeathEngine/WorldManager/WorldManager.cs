using CertainDeathEngine.DAL;
using CertainDeathEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.WorldManager
{
    public sealed class WorldManager
    {
        static readonly WorldManager _instance = new WorldManager();

        public static WorldManager Instance
        {
            get
            {
                return _instance;
            }
        }

        static Dictionary<int, GameWorldWrapper> worlds;

        WorldManager()
        {
            if (worlds == null)
            {
                worlds = new Dictionary<int, GameWorldWrapper>();
            }
        }

        public bool HasWorld(int worldId)
        {
            lock (worlds)
            {
                return (worlds.ContainsKey(worldId));
            }
        }

        public GameWorld GetWorld(int worldId)
        {
            lock (worlds)
            {
                if (worlds.ContainsKey(worldId))
                {
                    return worlds[worldId].World;
                }
                else
                {
                    return null;
                }
            }
        }

        public void KeepWorld(GameWorld world)
        {
            lock (worlds)
            {
                if (worlds.ContainsKey(world.Id))
                {
                    throw new Exception("The world is already loaded");
                }
                else
                {
                    worlds.Add(world.Id, new GameWorldWrapper() { World = world });
                }
            }
        }

        public IEnumerable<string> GetLoadedWorldIds()
        {
            return worlds.Keys.Select(x => x.ToString()).ToList();
        }
    }
}
