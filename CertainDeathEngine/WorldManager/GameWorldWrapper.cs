using CertainDeathEngine.Models;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.WorldManager
{
    [Serializable]
    public class GameWorldWrapper : Collection<GameWorld>
    {
        public GameWorld World { get; set; }
        //public long LastUpdateTime { get; set; }
        //public long LastSaveTime { get; set; }

        public void SetWorld(GameWorld world)
        {
            this.World = world;
        }


        public byte[] EFSerialized
        {
            get // serialize
            {
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
                    Console.WriteLine("Failed to serialize the world: {0}", e.Message);
                    return null;
                }
            }

            set // deserialize
            {
                try
                {
                    using (Stream s = new MemoryStream(value))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        GameWorld loadedWorld = (GameWorld)formatter.Deserialize(s);
                        World = loadedWorld;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to deserialize the world: {0}", e.Message);
                }
            }
        }
    }
}
