using CertainDeathEngine.DAL;
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
            Log.Info("Created AdminController");
            this._gameDal = gameDal;
            this._userDal = userDal;
            this._statisticsDal = statisticsDal;
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        //public ActionResult FBUsers()
        //{
        //    return View("FBUsers", _userDal.GetAllFbUsers());
        //}

        //public ActionResult Users()
        //{
        //    return View("Users", _userDal.GetAllUsers());
        //}

        //public ActionResult Worlds()
        //{
        //    return View("Worlds", _gameDal.GetWorldList());
        //}
    }
}