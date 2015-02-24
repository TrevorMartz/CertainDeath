using CertainDeathEngine.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CertainDeathEngine
{
    public class Updater
    {
        const int FRAME_TICK_COUNT = 16;
        private bool Running;
        private Game Game;
        private GameWorld World;
        private int LastTimeInMillis = 0;
        private int updateCount;
        private Random Rand;

        public Updater(Game g)
        {
            Trace.WriteLine(string.Format("Creating a new Updater thread: {0}", Thread.CurrentThread.ManagedThreadId));
            Running = false;
            Game = g;
            World = g.World;
            Rand = new Random();
        }


        public void Run()
        {
            Trace.WriteLine(string.Format("Starting thread: {0}", Thread.CurrentThread.ManagedThreadId));
            Running = true;
            updateCount = 1;

            while (Running)
            {
                while (GetCurrentTime() < LastTimeInMillis + FRAME_TICK_COUNT)
                {
                    // just wait
                }
                int curTime = GetCurrentTime();
                int delta = curTime - LastTimeInMillis;
                LastTimeInMillis = curTime;

                ProcessDeltaTime(delta);

                updateCount++;
            }
        }

        private void ProcessDeltaTime(int delta)
        {
            Trace.WriteLine(string.Format("Processing game {0} delta {1} on {2}", World.Id ,delta, Thread.CurrentThread.ManagedThreadId));
            // Now update everything

            lock (World.CurrentTile.Objects)
            {
                // update monsters
                foreach (GameObject obj in World.CurrentTile.Objects)
                {
                    // call the update method for the object
                }
            }

            lock (World.CurrentTile.Squares)
            {
                // check resources
                foreach (Square s in World.CurrentTile.Squares)
                {
                    if (s.Resource != null)
                    {
                        // do some math to figure out the respawn rate

                    }
                }
            }

            // add spawns

            // save everything
            if ((updateCount % 100) == 0)
            {
                Game.SaveWorld();
            }
        }

        public int GetCurrentTime()
        {
            return Environment.TickCount;
        }
    }
}
