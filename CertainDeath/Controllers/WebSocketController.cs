using CertainDeathEngine;
using CertainDeathEngine.DAL;
using Microsoft.Web.WebSockets;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Threading;
using log4net;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;
using CertainDeathEngine.Models.NPC.Buildings;
using System;
using System.Collections.Generic;
using CertainDeathEngine.Models;
using CertainDeathEngine.Models.NPC;

namespace CertainDeath.Controllers
{
    public class WebSocketController : ApiController
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IGameDAL _gameDal;
        private readonly IUserDAL _userDal;
        private readonly IStatisticsDAL _statisticsDal;

        public WebSocketController(IGameDAL gameDal, IUserDAL userDal, IStatisticsDAL statisticsDal)
        {
            Log.Debug("Created HomeController");
            _gameDal = gameDal;
            _userDal = userDal;
            _statisticsDal = statisticsDal;
        }

        // GET: WebSocket
        public HttpResponseMessage Get(int id)
        {
            Log.Debug("Creating WebSocketHandler for world " + id);
            HttpContext.Current.AcceptWebSocketRequest(new GameWebSocketHandler(_gameDal, _userDal, _statisticsDal, id));
            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }


        public class GameWebSocketHandler : WebSocketHandler
        {
            private IGameDAL _gameDal;
            private IUserDAL _userDal;
            private IStatisticsDAL _statisticsDal;
            protected int GameWorldId;
            protected EngineInterface GameInstance;
            private Thread _thisEndOfTheWebSocketTread;
            private bool Running;

            public GameWebSocketHandler(IGameDAL gameDal, IUserDAL userDal, IStatisticsDAL statisticsDal, int worldId)
            {
                Log.Debug("Created GameWebSocketHandler for world " + worldId);
                _gameDal = gameDal;
                _userDal = userDal;
                _statisticsDal = statisticsDal;
                this.GameWorldId = worldId;
                this.GameInstance = _gameDal.CreateGame(GameWorldId);
                Running = false;
            }

            public override void OnMessage(string message)
            {
                Log.Debug("OnMessage: " + message);
                dynamic result = JObject.Parse(message);

                if (result["event"] == "click")
                {
                    GameInstance.SquareClicked(new RowColumnPair((int)(float)result.y, (int)(float)result.x));
                }
                else if (result["event"] == "placeBuilding")
                {
                    string type = result["type"];
                    BuildingType btype = (BuildingType)Enum.Parse(typeof(BuildingType), type);
                    int x = result.x;
                    int y = result.y;
                    GameInstance.BuildBuildingAtSquare(y, x, btype);
                }
                else if (result["event"] == "moveTile")
                {
                    string direction = result["direction"];
                    string output = "";
                    switch (direction)
                    {
                        case "up":
                            output = GameInstance.MoveUp();
                            break;
                        case "down":
                            output = GameInstance.MoveDown();
                            break;
                        case "left":
                            output = GameInstance.MoveLeft();
                            break;
                        case "right":
                            output = GameInstance.MoveRight();
                            break;
                    }
                    Send(output);
                }
            }

            public override void OnOpen()
            {
                Running = true;
                _thisEndOfTheWebSocketTread = new Thread(SendUpdates);
                _thisEndOfTheWebSocketTread.Name = "thisEndOfTheWebSocketTread for world " + GameWorldId;
                _thisEndOfTheWebSocketTread.Start(); // Should be a safe call since the thread terminates when the connection closes
            }

            public void SendUpdates()
            {
                Log.Debug("Sending Updates");
                Log.Debug("Sending whole world");
                string jsonString = GameInstance.ToJSON();
                Send(jsonString);
                while (WebSocketContext.IsClientConnected && Running)
                {
                    try
                    {
                        Thread.Sleep(32);
                        Queue<UpdateMessage> tempUpdates;
                        lock (((Game) GameInstance).World.Updates)
                        {
                            tempUpdates = new Queue<UpdateMessage>(((Game) GameInstance).World.Updates);
                            ((Game) GameInstance).World.Updates.Clear();
                        }

                        bool sendAll = false;
                        //Trace.WriteLine("update count: " + tempUpdates.Count);
                        Log.Debug("there are " + tempUpdates.Count + " updates");
                        if (tempUpdates.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append("{\"updates\":[");
                            foreach (UpdateMessage m in tempUpdates)
                            {
                                if (m.GetType() == typeof(GameOverUpdateMessage) || m is GameOverUpdateMessage)
                                {
                                    Running = false;
                                    GameInstance.GameOver();
                                }
                                if (m.GetType() != typeof(WorldUpdateMessage) || !(m is WorldUpdateMessage))
                                {
                                    sb.Append(JsonConvert.SerializeObject(m));
                                }
                                else
                                {
                                    sendAll = true;
                                }
                                sb.Append(",");
                            }
                            sb.Remove(sb.Length - 1, 1);
                            sb.Append("]}");

                            jsonString = sb.ToString();

                            // comment the next line out
                            //sendAll = true;

                            if (sendAll)
                            {
                                jsonString = GameInstance.ToJSON();
                            }

                            // send it
                            Log.Debug(jsonString);
                            //Trace.WriteLine(jsonString);
                            Send(jsonString);
                        } // if tempUpdates > 0
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error on the listening thread: " + e.Message);
                    }
                }
                UpdateManager.Instance.RemoveGameThread(GameWorldId);
            }

            public override void OnClose()
            {
                Running = false;
                //_thisEndOfTheWebSocketTread.Abort();
                Log.Debug("thisEndOfTheWebSocketTread for world " + GameWorldId + " is aborting");
                Log.Debug("Connection closed");
            }
        }
    }
}