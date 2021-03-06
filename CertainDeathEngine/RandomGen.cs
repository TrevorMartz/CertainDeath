﻿using System;
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
            Log.Debug("Initializing seeded random number generator");
            string randomSeed = ConfigurationManager.AppSettings["RandomNumberSeed"];
            Log.Debug("Using RandomNumberSeed: " + randomSeed);
            try
            {
                Random = new Random(Convert.ToInt32(randomSeed));
            }
            catch (Exception)
            {
                Random = new Random();
            }
        }

        public static void InitUnseeded()
        {
            Log.Debug("Initializing unseeded random number generator");
            Random = new Random();
        }
    }
}
