using CertainDeathEngine.Models.User;
using System.Collections.Generic;
using System.Linq;

namespace CertainDeathEngine.DAL
{
    public interface IStatisticsDAL
    {
        void SaveScore(Score score);

        IQueryable<Score> GetScoresForUser(int userId);

        Dictionary<Score, MyAppUser> GetHighScores(int qty);
    }
}
