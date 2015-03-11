﻿using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC;
using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using CertainDeathEngine.DAL;
using CertainDeathEngine.Models.Resources;
using System.Windows;

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
            Log.Debug(string.Format("Creating a new Updater thread: {0}", Thread.CurrentThread.ManagedThreadId));
            Running = false;
            _game = game;
        }

        public void Run()
        {
            Log.Debug(string.Format("Starting thread: {0}", Thread.CurrentThread.ManagedThreadId));
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
            lock (_game.World)
            {
                // update monsters
                foreach (Tile t in _game.World.Tiles)
                {
                    IEnumerable<Temporal> timeObjects = new List<Temporal>(t.Objects.OfType<Temporal>());
                    foreach (Temporal tim in timeObjects)
                    {
                        tim.Update(delta);
                    }
                }
            }

            lock (_game.World)
            {
                // process clicks
                Queue<RowColumnPair> tempClicks;
                lock (_game.World.SquareClicks)
                {
                    tempClicks = new Queue<RowColumnPair>(_game.World.SquareClicks);
                    _game.World.SquareClicks.Clear();
                }

                if (tempClicks.Count > 0)
                {
                    foreach (var click in tempClicks)
                    {
                        Square square = _game.World.CurrentTile.Squares[click.Column, click.Row];
                        if (square.Resource != null)
                        {
                            ResourceType curType = square.Resource.Type;
                            int gathered = square.GatherResource();

                            _game.World.Player.AddResource(curType, gathered);
                            _game.World.Score.AddResource(curType, gathered);
                            _game.World.AddUpdateMessage(new AddResourceToPlayerUpdateMessage(_game.World.Player.Id)
                                                         {
                                                             ResourceType = curType.ToString(),
                                                             Amount = gathered
                                                         });
                        }
                    }
                }
            }

            // add spawns
            _game.MonsterGenerator.Update(delta);

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
