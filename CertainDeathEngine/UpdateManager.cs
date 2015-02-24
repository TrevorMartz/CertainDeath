using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CertainDeathEngine
{
    public sealed class UpdateManager
    {
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
    }
}
