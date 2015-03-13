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
            List<CertainDeathUser> users = _cdDbModel.Users.Include("FBUser").ToList();
            return _cdDbModel.Scores
                .ToList()
                .OrderByDescending(x=>x.TotalResources)
                .ToDictionary(score => score, score => users.Single(u => u.Id == score.UserId).FBUser);
        }
    }
}
