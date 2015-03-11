using CertainDeath.Models;
using CertainDeathEngine;
using CertainDeathEngine.DAL;
using CertainDeathEngine.Models.User;
using log4net;
using Microsoft.AspNet.Facebook;
using Microsoft.AspNet.Facebook.Client;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CertainDeath.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IGameDAL _gameDal;
        private readonly IUserDAL _userDal;
        private readonly IStatisticsDAL _statisticsDal;

        public HomeController(IGameDAL gameDal, IUserDAL userDal, IStatisticsDAL statisticsDal)
        {
            Log.Debug("Created HomeController");
            _gameDal = gameDal;
            _userDal = userDal;
            _statisticsDal = statisticsDal;
        }

        [FacebookAuthorize("user_photos")]
        public async Task<ActionResult> Index(FacebookContext context)
        {
            Log.Debug("Loading Index on HomeController");
            if (ModelState.IsValid)
            {
                Log.Debug("Authenticating with Facebook");
                var facebookUser = await context.Client.GetCurrentUserAsync<MyAppUser>();
                Log.Debug("Found user: " + facebookUser.Id + " - " + facebookUser.Name);

                Log.Debug("Loading CDUser");
                var certainDeathUser = _userDal.GetGameUser(facebookUser);

                if (certainDeathUser == null)
                {
                    Log.Debug("CDUser does not exist.  Redirecting to reigstration page");
                    // They have authenicated with FaceBook, but they have never played our game
                    // We need them to pick a username
                    return GameRegistration(facebookUser);
                }

                Log.Debug("Found user with id: " + certainDeathUser.Id);
                // We have a user, play the game
                return RedirectToAction("StartScreen", certainDeathUser);
            }

            // something was wrong
            return View("Error");
        }

        // Take a facebook user and make a CDUser
        public ActionResult GameRegistration(MyAppUser fbUser)
        {
            if (fbUser == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CertainDeathUser certainDeathUser = _userDal.CreateGameUser(fbUser);
            return View("GameRegistration", certainDeathUser);
        }

        // Save the CDUser
        [HttpPost]
        public ActionResult GameRegistration(CertainDeathUser cdUser)
        {
            if (ModelState.IsValid)
            {
                cdUser.CreateTime = Environment.TickCount;
                cdUser.LastLoginTime = Environment.TickCount;
                _userDal.UpdateGameUser(cdUser);
                return RedirectToAction("Index");
            }
            return View("GameRegistration", cdUser);
        }

        // Load the start screen
        public ActionResult StartScreen(CertainDeathUser cdUser)
        {
            StartScreenViewModel ssvm = new StartScreenViewModel(cdUser.Id);

            if (cdUser.WorldId > 0)
            {
                ssvm.LoadGame = true;
            }
            return View("StartScreen", ssvm);
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

        public ActionResult Leaderboard()
        {
                return PartialView("_LeaderBoard", _statisticsDal.GetHighScores(10));
        }

        public ActionResult HowToPlay()
        {
            return PartialView("_HowToPlay");
        }

        public ActionResult LoadGame(int userid)
        {
            CertainDeathUser cdUser = _userDal.GetGameUser(userid);
            return View("Game", cdUser);
        }

        public ActionResult NewGame(int userid)
        {

            CertainDeathUser cdUser = _userDal.GetGameUser(userid);

            // we need to create them a game world
            Game game = (Game)_gameDal.CreateGame();
            game.World.Score.UserId = cdUser.Id;
            _userDal.GiveGameUserAGameWorldId(cdUser.Id, game.World.Id);
            return View("Game", cdUser);
        }
    }
}
