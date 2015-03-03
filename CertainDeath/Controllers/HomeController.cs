﻿using CertainDeathEngine;
using CertainDeathEngine.DAL;
using CertainDeathEngine.Models.User;
using log4net;
using Microsoft.AspNet.Facebook;
using Microsoft.AspNet.Facebook.Client;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CertainDeath.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IGameDAL _gameDal;
        private readonly IUserDAL _userDal;

        public HomeController(IGameDAL gameDal, IUserDAL userDal)
        {
            Log.Info("Created HomeController");
            this._gameDal = gameDal;
            this._userDal = userDal;
        }

        [FacebookAuthorize("email", "user_photos")]
        public async Task<ActionResult> Index(FacebookContext context)
        {
            if (ModelState.IsValid)
            {
                var facebookUser = await context.Client.GetCurrentUserAsync<MyAppUser>();

                // Below is some default code.  I dont know if we will use it.
                if (Request.IsAjaxRequest())
                    return PartialView("_FriendView", facebookUser);

                var certainDeathUser = _userDal.GetGameUser(facebookUser);

                if (certainDeathUser == null)
                {
                    // TODO: return a registration page where they can pick a name or something
                    //return View("GameRegistration", facebookUser);
                    certainDeathUser = _userDal.CreateGameUser(facebookUser);
                }

                // We have a user, play the game
                if (certainDeathUser.WorldId < 1)
                {
                    // we need to create them a game world
                    Game game = (Game)_gameDal.CreateGame();
                    _userDal.GiveGameUserAGameWorldId(certainDeathUser.Id, game.World.Id);
                    
                }
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
            // TODO: Do we need this action for anything?
            if (ModelState.IsValid)
            {
                return View(context);
            }

            return View("Error");
        }
    }
}
