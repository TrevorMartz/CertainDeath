using Microsoft.Web.WebSockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using CertainDeathEngine.DAL;
using CertainDeathEngine.Models.User;
using System.Diagnostics;
using System.Web.Hosting;

namespace CertainDeath.Controllers
{
    public class WebSocketController : ApiController
    {
        // GET: WebSocket
        public HttpResponseMessage Get(int id)
        {
            // pass in the world id
            HttpContext.Current.AcceptWebSocketRequest(new GameWebSocketHandler(id));
            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }

        public class GameWebSocketHandler : WebSocketHandler
        {

            protected int GameWorldId;
            protected IGameDAL GameDAL;
            protected IUserDAL UserDAL;

            public GameWebSocketHandler(int worldId)
            {
                this.GameWorldId = worldId;
                GameDAL = new BasicGameDAL(HostingEnvironment.MapPath("~\\Data"));
                UserDAL = new BasicUserDAL(HostingEnvironment.MapPath("~\\Data"));
            }

            public override void OnMessage(string message)
            {
                Trace.WriteLine(message);
            }

            public override void OnOpen()
            {
                // we already know the world id so I dont think we need to ask again
                //CertainDeathUser user = UserDAL.GetGameUser(null);// need to pass in some fb context
                Send(GameDAL.LoadGame(GameWorldId).ToJSON());
            }

            public override void OnClose()
            {
                Trace.WriteLine("[Connection closed]");
            }
        }
    }
}