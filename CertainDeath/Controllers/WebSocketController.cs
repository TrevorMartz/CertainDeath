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
            HttpContext.Current.AcceptWebSocketRequest(new GameWebSocketHandler(id));
            return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
        }

        public class GameWebSocketHandler : WebSocketHandler
        {
            private int id;
            IGameDAL GameDAL = new BasicGameDAL(HostingEnvironment.MapPath("~\\Data"));
            IUserDAL UserDAL = new BasicUserDAL();

            public GameWebSocketHandler(int id)
            {
                this.id = id;
            }
            public override void OnMessage(string message)
            {
                Trace.WriteLine(message);
            }

            public override void OnOpen()
            {
                CertainDeathUser user = UserDAL.GetUser(null);// need to pass in some fb context
                Send(GameDAL.LoadGame(user.WorldId).ToJSON());
            }

            public override void OnClose()
            {
                Trace.WriteLine("[Connection closed]");
            }
        }
    }
}