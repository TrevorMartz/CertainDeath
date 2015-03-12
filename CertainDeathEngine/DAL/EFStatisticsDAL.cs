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
            score.SaveDate = Environment.TickCount;
            _cdDbModel.Scores.Add(score);
            _cdDbModel.SaveChanges();
        }

        public IQueryable<Score> GetScoresForUser(int userId)
        {
            return _cdDbModel.Scores.Where(x => x.UserId == userId);
        }

        public IEnumerable<Score> GetHighScores(int qty)
        {
            //High score will be determined by time survived. The rest are just stats.
            return _cdDbModel.Scores.OrderBy(x => x.Kills).ToList();
        }
    }
}
