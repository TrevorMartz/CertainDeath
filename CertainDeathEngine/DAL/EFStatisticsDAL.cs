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
    }
}
