using CertainDeathEngine.DB;
using CertainDeathEngine.Models.User;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CertainDeathEngine.DAL
{
    public class EFStatisticsDAL : IStatisticsDAL
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CDDBModel _cdDbModel;

        public EFStatisticsDAL()
        {
            _cdDbModel = new CDDBModel();
        }

        public void SaveScore(Score score)
        {
            score.SaveDate = DateTime.Now.Ticks;
            _cdDbModel.Scores.Add(score);
            _cdDbModel.SaveChanges();
        }

        public IQueryable<Score> GetScoresForUser(int userId)
        {
            return _cdDbModel.Scores.Where(x => x.UserId == userId);
        }

        public Dictionary<Score, MyAppUser> GetHighScores(int qty)
        {
            var users = _cdDbModel.Users.Include("FBUser").ToList();
            Dictionary<Score, MyAppUser> userScores = new Dictionary<Score, MyAppUser>();
            foreach (var score in _cdDbModel.Scores.ToList())
                userScores.Add(score, users.Where(u => u.Id == score.UserId).FirstOrDefault().FBUser);
            return userScores;

            //return _cdDbModel.Scores.OrderByDescending(x => x.Kills).ToList();
        }
    }
}
