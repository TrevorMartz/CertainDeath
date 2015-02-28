using CertainDeathEngine.Models;
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


        public string EFSerialized
        {
            get // serialize
            {
                //string retVal = "";
                //using (var ms = SerializeToStream(World))
                //{
                //    ms.Position = 0;
                //    var sr = new StreamReader(ms);
                //    retVal = sr.ReadToEnd();
                //}
                //return retVal;
                string retVal = Newtonsoft.Json.JsonConvert.SerializeObject(World);
                return retVal;
            }

            set // deserialize
            {

                ////return;
                //GameWorld loadedWorld = null;
                //using (Stream s = GenerateStreamFromString(value))
                //{
                //    BinaryFormatter formatter = new BinaryFormatter();
                //    loadedWorld = (GameWorld)formatter.Deserialize(s);
                //}
                //World = loadedWorld;



                // right now the json deserialization failes, so just return
                // TODO: fix this
                return;

                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                var jData = Newtonsoft.Json.JsonConvert.DeserializeObject<GameWorld>(value);
                this.World = jData;
            }
        }
        public static MemoryStream SerializeToStream(object o)
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);
            return stream;
        }

        public static object DeserializeFromStream(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object o = formatter.Deserialize(stream);
            return o;
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
