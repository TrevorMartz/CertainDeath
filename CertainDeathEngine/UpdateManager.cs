using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CertainDeathEngine
{
    public sealed class UpdateManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static readonly UpdateManager _instance = new UpdateManager();
        public static UpdateManager Instance
        {
            get
            {
                return _instance;
            }
        }

        static Dictionary<int, Thread> threads;

        UpdateManager()
        {
            if (threads == null)
                threads = new Dictionary<int, Thread>();
        }

        public void AddGameThread(int worldId, Thread thread)
        {
            lock (threads)
            {
                if (threads.ContainsKey(worldId))
                {
                    if (threads[worldId].IsAlive)
                    {
                        // thread is already there and running
                        return;
                    }
                    else
                    {
                        threads[worldId].Abort();
                        threads[worldId] = thread;
                    }
                }
                else
                {
                    threads[worldId] = thread;
                }
                threads[worldId].Start();
            }
        }

        public void RemoveGameThread(int worldId)
        {
            lock (threads)
            {
                if (threads.ContainsKey(worldId))
                {
                    threads[worldId].Abort();
                    threads.Remove(worldId);
                }
                else
                {
                    // it wasnt there.
                    return;
                }
            }
        }

        public IEnumerable<string> GetUpdatingWorldIds()
        {
            return threads.Keys.Select(x => x.ToString()).ToList();
        }
    }
}
