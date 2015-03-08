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
            Log.Info("Created HomeController");
            _gameDal = gameDal;
            _userDal = userDal;
            _statisticsDal = statisticsDal;
        }

        // GET: WebSocket
        public HttpResponseMessage Get(int id)
        {
            Log.Info("Creating WebSocketHandler for world " + id);
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
                Log.Info("Created GameWebSocketHandler for world " + worldId);
                _gameDal = gameDal;
                _userDal = userDal;
                _statisticsDal = statisticsDal;
                this.GameWorldId = worldId;
                this.GameInstance = _gameDal.CreateGame(GameWorldId);
                Running = false;
            }

            public override void OnMessage(string message)
            {
                //JsonConvert.PopulateObject(message, new ScreenRequest());
                dynamic result = JObject.Parse(message);

                if (result["event"] == "click")
                {
                    GameInstance.SquareClicked((float)result.x, (float)result.y);
                }
                else if (result["event"] == "placeBuilding")
                {
                    string type = result["type"];
                    BuildingType btype = (BuildingType)Enum.Parse(typeof(BuildingType), type);
                    int x = result.x;
                    int y = result.y;
                    GameInstance.BuildBuildingAtSquare(y, x, btype);
                }
                //else if (result["event"] == "moveTile")
                //{
                //    string direction = result["direction"];
                //    switch (direction)
                //    {
                //        case "up":
                //            GameInstance.MoveUp();
                //            break;
                //        case "down":
                //            GameInstance.MoveDown();
                //            break;
                //        case "left":
                //            GameInstance.MoveLeft();
                //            break;
                //        case "right":
                //            GameInstance.MoveRight();
                //            break;
                //    }
                //}
                Log.Info("OnMessage: " + message);
                Trace.WriteLine(message);
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
                string jsonString = GameInstance.ToJSON();
                Send(jsonString);
                while (WebSocketContext.IsClientConnected && Running)
                {
                    try
                    {
                        Thread.Sleep(32);
                        lock (((Game) GameInstance).World)
                        {
                            bool sendAll = false;
                            if (((Game) GameInstance).World.Updates.Count > 0)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append("\"updates\":[");
                                foreach (var m in ((Game) GameInstance).World.Updates)
                                {
                                    if (m.GetType() != typeof (WorldUpdateMessage))
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
                                sb.Append("]");

                                ((Game) GameInstance).World.Updates.Clear();

                                //jsonString = sb.ToString();
                                //Trace.WriteLine(jsonString);
                                //Send(jsonString);

                                //if (sendAll)
                                //{
                                //    // send the whole world for now
                                    jsonString = GameInstance.ToJSON();
                                    Send(jsonString);
                                //}
                            }
                        }
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
                Log.Info("thisEndOfTheWebSocketTread for world " + GameWorldId + " is aborting");
                Log.Info("Connection closed");
                Trace.WriteLine("[Connection closed]");
            }
        }
    }
}