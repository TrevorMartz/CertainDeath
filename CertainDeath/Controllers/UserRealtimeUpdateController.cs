using System;
using System.Configuration;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNet.Facebook.Models;
using Microsoft.AspNet.Facebook.Realtime;

// To learn more about Facebook Realtime Updates, go to http://go.microsoft.com/fwlink/?LinkId=301876

namespace CertainDeath.Controllers
{

    // TODO: do we need this controller?

    public class UserRealtimeUpdateController : FacebookRealtimeUpdateController
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly static string UserVerifyToken = ConfigurationManager.AppSettings["Facebook:VerifyToken:User"];

        public override string VerifyToken
        {
            get
            {
                return UserVerifyToken;
            }
        }

        public override Task HandleUpdateAsync(ChangeNotification notification)
        {
            if (notification.Object == "user")
            {
                foreach (var entry in notification.Entry)
                {
                    // Your logic to handle the update here
                }
            }

            throw new NotImplementedException();
        }
    }
}
