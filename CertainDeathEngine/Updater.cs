using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CertainDeathEngine.DAL;

namespace CertainDeathEngine
{
    public class Updater
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IGameDAL _gameDal = new EFGameDAL();
        const int FRAME_TICK_COUNT = 16;
        public bool Running;
        private readonly Game _game;
        private int _lastTimeInMillis;
        private int _updateCount;

        public Updater(Game game)
        {
            Log.Info(string.Format("Creating a new Updater thread: {0}", Thread.CurrentThread.ManagedThreadId));
            Running = false;
            _game = game;
        }


        public void Run()
        {
            Trace.WriteLine(string.Format("Starting thread: {0}", Thread.CurrentThread.ManagedThreadId));
            Running = true;
            _updateCount = 1;

            _lastTimeInMillis = GetCurrentTime() - FRAME_TICK_COUNT - FRAME_TICK_COUNT;
            while (Running && (_lastTimeInMillis - _game.World.TimeLastQueried) > 3 * 60 * 100 * 1000) // three minutes
            {
                while (GetCurrentTime() < _lastTimeInMillis + FRAME_TICK_COUNT)
                {
                    // just wait
                }
                int curTime = GetCurrentTime();
                int delta = curTime - _lastTimeInMillis;
                _lastTimeInMillis = curTime;

                ProcessDeltaTime(delta);
                //todo: lock this?
                _game.World.TimeLastUpdated = _lastTimeInMillis;

                _updateCount++;
            }
        }

        private void ProcessDeltaTime(int delta)
        {
            //Trace.WriteLine(string.Format("Processing game {0} delta {1} on {2}", World.Id ,delta, Thread.CurrentThread.ManagedThreadId));
            // Now update everything

            lock (_game.World)
            {
                // update monsters
                foreach (Tile t in _game.World.Tiles)
                {
                    IEnumerable<Temporal> timeObjects = new List<Temporal>(t.Objects.OfType<Temporal>());
                    foreach (Temporal tim in timeObjects)
                        tim.Update(delta);
                }
            }

            lock (_game.World)
            {
                // check resources
                foreach (Square s in _game.World.CurrentTile.Squares)
                {
                    if (s.Resource != null)
                    {
                        // do some math to figure out the respawn rate

                    }
                }
            }

            // add spawns
            //g.MonsterGenerator.Update(500);

            // save everything
            if ((_updateCount % 1000) == 0) // about one minute
            {
                _gameDal.SaveGame(_game);
            }
        }

        public int GetCurrentTime()
        {
            return Environment.TickCount;
        }
    }
}
