using CertainDeathEngine.Models;
using log4net;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CertainDeathEngine.WorldManager
{
    [Serializable]
    public class GameWorldWrapper : Collection<GameWorld>
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public GameWorld World { get; set; }
        public bool HasEnded { get; set; }
        public long LastUpdateTime { get; set; }
        public long LastSaveTime { get; set; }


        public byte[] EfSerialized
        {
            get // serialize
            {
                //HasEnded = World.HasEnded;
                Log.Debug("Serializing game world with id " + World.Id);
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        IFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(ms, World);
                        byte[] retVal = ms.ToArray();
                        return retVal;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Failed to serialize the world: " + e.Message);
                    return null;
                }
            }

            set // deserialize
            {
                Log.Debug("Deserializing a game world.  I dont yet know the id");
                try
                {
                    using (Stream s = new MemoryStream(value))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        GameWorld loadedWorld = (GameWorld)formatter.Deserialize(s);
                        World = loadedWorld;
                        //HasEnded = loadedWorld.HasEnded;
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Failed to deserialize the world: " + e.Message);
                }
            }
        }
    }
}
