using CertainDeathEngine.Models.User;
using System.Linq;

namespace CertainDeathEngine.DAL
{
    public interface IStatisticsDAL
    {
        void SaveScore(Score score);

        IQueryable<Score> GetScoresForUser(int userId);
    }
}
