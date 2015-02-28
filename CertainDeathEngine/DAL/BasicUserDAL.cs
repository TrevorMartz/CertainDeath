using CertainDeathEngine.Models.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace CertainDeathEngine.DAL
{
    public class BasicUserDAL : IUserDAL
    {
        private string FilePath;
        private List<CertainDeathUser> users;

        public BasicUserDAL(string path)
        {
            FilePath = String.Format("{0}\\User", path);
            users = Load();
        }

        public CertainDeathUser CreateGameUser(MyAppUser fbUser)
        {
            if (users.Where(x => x.FBUser.Email.Equals(fbUser.Email)).Count() == 0)
            {
                CertainDeathUser newUser = new CertainDeathUser() { FBUser = fbUser, WorldId = -1 };
                users.Add(newUser);
                Save();
                return newUser;
            }
            else
            {
                // the user already existed, so return the existing user
                return GetGameUser(fbUser);
                // TODO - Unless we want to trow exceptions for someone else to deal with - blake
            }
        }


        public CertainDeathUser GetGameUser(MyAppUser fbUser)
        {
            return (users.Where(x => x.FBUser.Id.Equals(fbUser.Id)).FirstOrDefault());
        }

        private void Save()
        {
            try
            {
                Serialize(users, "users.bin");
            }
            catch (Exception)
            {
                // TODO: do something about this exception
            }
        }

        private List<CertainDeathUser> Load()
        {
            return Deserialize("users.bin");
        }

        private void Serialize(List<CertainDeathUser> list, string filename)
        {
            System.IO.Stream ms = File.OpenWrite(String.Format("{0}\\{1}", FilePath, filename));
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, list);
            ms.Flush();
            ms.Close();
            ms.Dispose();
        }

        private List<CertainDeathUser> Deserialize(string filename)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream fs = File.Open(String.Format("{0}\\{1}", FilePath, filename), FileMode.Open);

                object obj = formatter.Deserialize(fs);
                List<CertainDeathUser> list = (List<CertainDeathUser>)obj;
                fs.Flush();
                fs.Close();
                fs.Dispose();
                return list;
            }
            catch (Exception)
            {
                // TODO do we want to do something better for this exception?
                return new List<CertainDeathUser>();
            }
        }

    }
}
