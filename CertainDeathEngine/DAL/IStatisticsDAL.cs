using CertainDeathEngine.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.DAL
{
    public interface IStatisticsDAL
    {
        void SaveScore(Score score);
        IEnumerable<Score> GetScoresForUser(int userId);
    }
}
