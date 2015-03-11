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
                Log.Debug("inside the threads lock");
                if (_threads.ContainsKey(worldId))
                {
                    Log.Debug("update thread for world is already running.");
                    if (_threads[worldId].IsAlive)
                    {
                        Log.Debug("returning cause it is alive");
                        // thread is already there and running
                        return;
                    }
                    else
                    {
                        Log.Debug("Updater thread exists but is not alive, aborting bad thread " + _threads[worldId].ManagedThreadId);
                        _threads[worldId].Abort();
                        Log.Debug("Keeping new thread");
                        _threads[worldId] = thread;
                    }
                }
                else
                {
                    Log.Debug("Keeping new thread");
                    _threads[worldId] = thread;
                }
                Log.Debug("starting the thread");
                _threads[worldId].Start();
            }
        }

        public void RemoveGameThread(int worldId)
        {
            Log.Debug("Removing game thread");
            lock (_threads)
            {
                Log.Debug("inside the threads lock");
                if (_threads.ContainsKey(worldId))
                {
                    Log.Debug("update thread for world is already running. aborting the thread "+ _threads[worldId].ManagedThreadId);
                    _threads[worldId].Abort();
                    Log.Debug("removing the thread from the collection.");
                    _threads.Remove(worldId);
                }
                else
                {
                    Log.Debug("There is no thread to remove.");
                }
            }
        }

        public bool HasWorld(int worldId)
        {
            lock (_threads)
            {
                return (_threads.ContainsKey(worldId));
            }
        }

        public IEnumerable<string> GetUpdatingWorldIds()
        {
            return _threads.Keys.Select(x => x.ToString()).ToList();
        }
    }
}
