using CertainDeathEngine.DB;
using CertainDeathEngine.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public IEnumerable<Score> GetScoresForUser(int userId)
        {
            return cdDBModel.Scores.Where(x => x.UserId == userId);
        }
    }
}
