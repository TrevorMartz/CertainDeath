using System;
using System.Configuration;
using log4net;

namespace CertainDeathEngine
{
    public class RandomGen
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Random Random { get; private set; }

        public static void Init()
        {
            string s = ConfigurationManager.AppSettings["RandomNumberSeed"];
            try
            {
                Random = new Random(Convert.ToInt32(s));
            }
            catch (Exception)
            {
                Random = new Random();
            }
        }

        public static void InitUnseeded()
        {
            Random = new Random();
        }
    }
}
