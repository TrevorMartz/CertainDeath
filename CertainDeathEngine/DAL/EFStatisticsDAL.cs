using CertainDeathEngine.DB;
using CertainDeathEngine.Models.User;
using System.Linq;

namespace CertainDeathEngine.DAL
{
    public class EFStatisticsDAL : IStatisticsDAL
    {
        private CDDBModel cdDBModel;

        public EFStatisticsDAL()
        {
            cdDBModel = new CDDBModel();
        }

        public void SaveScore(Score score)
        {
            cdDBModel.Scores.Add(score);
        }

        public IQueryable<Score> GetScoresForUser(int userId)
        {
            return cdDBModel.Scores.Where(x => x.UserId == userId);
        }

        public IQueryable<Score> GetHighScores(int qty)
        {
            //High score will be determined by time survived. The rest are just stats.
            return cdDBModel.Scores.OrderBy(x => x.Survived).Take(qty);
        }
    }
}
