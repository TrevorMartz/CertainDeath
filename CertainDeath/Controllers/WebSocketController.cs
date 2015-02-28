﻿using CertainDeathEngine;
using CertainDeathEngine.DAL;
using Microsoft.Web.WebSockets;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web;
//using System.Web.Hosting;
using System.Web.Http;

namespace CertainDeath.Controllers
{
    public class WebSocketController : ApiController
    {
        private readonly IGameDAL _gameDal;
        private readonly IUserDAL _userDal;
        private readonly IStatisticsDAL _statisticsDal;

        public WebSocketController(IGameDAL gameDal, IUserDAL userDal, IStatisticsDAL statisticsDal)
        {
            this._gameDal = gameDal;
            this._userDal = userDal;
            this._statisticsDal = statisticsDal;
        }

        // GET: WebSocket
        public HttpResponseMessage Get(int id)
        {
            // pass in the world id
            HttpContext.Current.AcceptWebSocketRequest(new GameWebSocketHandler(_gameDal, _userDal, _statisticsDal, id));
            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }


        public class GameWebSocketHandler : WebSocketHandler
        {
            private IGameDAL GameDAL;
            private IUserDAL UserDAL;
            private IStatisticsDAL StatisticsDAL;
            protected int gameWorldId;
            protected EngineInterface gameInstance;

            public GameWebSocketHandler(IGameDAL gameDal, IUserDAL userDal, IStatisticsDAL statisticsDal, int worldId)
            {
                GameDAL = gameDal;
                UserDAL = userDal;
                StatisticsDAL = statisticsDal;
                this.gameWorldId = worldId;
                this.gameInstance = GameDAL.CreateGame(gameWorldId);
            }

            public override void OnMessage(string message)
            {
                Trace.WriteLine(message);
            }

            public override void OnOpen()
            {
                // we already know the world id so I dont think we need to ask again
                //CertainDeathUser user = UserDAL.GetGameUser(null);// need to pass in some fb context

                Send(gameInstance.ToJSON());

            }

            public override void OnClose()
            {
                Trace.WriteLine("[Connection closed]");
            }
        }
    }
}