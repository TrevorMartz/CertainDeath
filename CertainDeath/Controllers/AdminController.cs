using CertainDeathEngine.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CertainDeath.Controllers
{
    public class AdminController : Controller
    {
        private readonly IGameDAL _gameDal;
        private readonly IUserDAL _userDal;
        private readonly IStatisticsDAL _statisticsDal;

        public AdminController(IGameDAL gameDal, IUserDAL userDal, IStatisticsDAL statisticsDal)
        {
            this._gameDal = gameDal;
            this._userDal = userDal;
            this._statisticsDal = statisticsDal;
        }

        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Users()
        {
            return View("Users", _userDal.GetAllUsers());
        }
    }
}