using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Facebook;
using Microsoft.AspNet.Facebook.Client;
using CertainDeathEngine.Models.User;
using CertainDeathEngine.DAL;
using System.Web.Hosting;
//using CertainDeath.Models;

namespace CertainDeath.Controllers
{
    public class HomeController : Controller
    {
        protected IGameDAL GameDAL;
        protected IUserDAL UserDAL;

        public HomeController()
        {
            GameDAL = new BasicGameDAL(HostingEnvironment.MapPath("~\\Data"));
            UserDAL = new BasicUserDAL(HostingEnvironment.MapPath("~\\Data"), GameDAL);
        }

        [FacebookAuthorize("email", "user_photos")]
        public async Task<ActionResult> Index(FacebookContext context)
        {
            if (ModelState.IsValid)
            {
                var facebookUser = await context.Client.GetCurrentUserAsync<MyAppUser>();
                // I dont know what we want to return when the user has not created a world before,
                // maybe a null and then to a registration page
                var certainDeathUser = UserDAL.GetGameUser(facebookUser);

                // Below is some default code.  I dont know if we will use it.
                if (Request.IsAjaxRequest())
                    return PartialView("_FriendView", facebookUser);

                if (certainDeathUser == null)
                {
                    // do we want a registration page?
                    //return View("GameRegistration", facebookUser);
                    certainDeathUser = UserDAL.CreateGameUser(facebookUser);
                }
                
                // We have a user, play the game
                return View("Game", certainDeathUser);
                
            }

            // something was wrong
            return View("Error");
        }

        // This action will handle the redirects from FacebookAuthorizeFilter when
        // the app doesn't have all the required permissions specified in the FacebookAuthorizeAttribute.
        // The path to this action is defined under appSettings (in Web.config) with the key 'Facebook:AuthorizationRedirectPath'.
        public ActionResult Permissions(FacebookRedirectContext context)
        {
            if (ModelState.IsValid)
            {
                return View(context);
            }

            return View("Error");
        }
    }
}
