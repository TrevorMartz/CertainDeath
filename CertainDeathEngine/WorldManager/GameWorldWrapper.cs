using CertainDeathEngine.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
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


        public string EFSerialized
        {
            get // serialize
            {
                string retVal = "";
                using (var ms = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ms, this);
                    ms.Flush();

                    ms.Position = 0;
                    var sr = new StreamReader(ms);
                    retVal = sr.ReadToEnd();
                }
                return retVal;
            }

            set // deserialize
            {
                GameWorld loadedWorld = null;
                using (Stream s = GenerateStreamFromString(value))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    loadedWorld = (GameWorld)formatter.Deserialize(s);
                }
                World = loadedWorld;
            }
        }

        private Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
