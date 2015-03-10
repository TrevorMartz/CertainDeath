using CertainDeath.Models;
using CertainDeathEngine;
using CertainDeathEngine.DAL;
using CertainDeathEngine.WorldManager;
using log4net;
using System.Web.Mvc;

namespace CertainDeath.Controllers
{
    public class AdminController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IGameDAL _gameDal;
        private readonly IUserDAL _userDal;
        private readonly IStatisticsDAL _statisticsDal;

        public AdminController(IGameDAL gameDal, IUserDAL userDal, IStatisticsDAL statisticsDal)
        {
            Log.Debug("Created AdminController");
            _gameDal = gameDal;
            _userDal = userDal;
            _statisticsDal = statisticsDal;
        }

        public ActionResult Index()
        {
            if (Request.IsAjaxRequest())
            {
                AdminViewModel avm = new AdminViewModel()
                {
                    LoadedWorlds = WorldManager.Instance.GetLoadedWorldIds(),
                    UpdateThreads = UpdateManager.Instance.GetUpdatingWorldIds(),
                    Users = _userDal.GetAllUsers()
                };

                return PartialView("Index", avm);
            }
            return new RedirectResult("/");
        }
    }
}