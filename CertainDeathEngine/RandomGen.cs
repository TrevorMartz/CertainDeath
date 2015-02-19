using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;

namespace CertainDeathEngine
{
    public class RandomGen
    {
        public static Random Random { get; set; }

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
    }
}
