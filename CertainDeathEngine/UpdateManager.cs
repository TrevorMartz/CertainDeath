using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace CertainDeathEngine
{
    public sealed class UpdateManager
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly UpdateManager _instance = new UpdateManager();
        public static UpdateManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private static Dictionary<int, Thread> _threads;

        UpdateManager()
        {
            Log.Debug("Creating Singlton UpdateManager");
            if (_threads == null)
                _threads = new Dictionary<int, Thread>();
            else
            {
                throw new Exception("UpdateManager is a singlton.  Dont make it !!!!!!");
            }
        }

        public void AddGameThread(int worldId, Thread thread)
        {
            Log.Debug("Adding new game thread");
            lock (_threads)
            {
                if (_threads.ContainsKey(worldId))
                {
                    if (_threads[worldId].IsAlive)
                    {
                        // thread is already there and running
                        return;
                    }
                    else
                    {
                        _threads[worldId].Abort();
                        _threads[worldId] = thread;
                    }
                }
                else
                {
                    _threads[worldId] = thread;
                }
                _threads[worldId].Start();
            }
        }

        public void RemoveGameThread(int worldId)
        {
            lock (_threads)
            {
                if (_threads.ContainsKey(worldId))
                {
                    _threads[worldId].Abort();
                    _threads.Remove(worldId);
                }
                else
                {
                    // it wasnt there.
                }
            }
        }

        public IEnumerable<string> GetUpdatingWorldIds()
        {
            return _threads.Keys.Select(x => x.ToString()).ToList();
        }
    }
}
