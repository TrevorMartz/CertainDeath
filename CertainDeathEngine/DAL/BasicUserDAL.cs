using CertainDeathEngine.Models.User;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace CertainDeathEngine.DAL
{
    public class BasicUserDAL : IUserDAL
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _filePath;
        private readonly List<CertainDeathUser> _users;

        public BasicUserDAL(string path)
        {
            _filePath = String.Format("{0}\\User", path);
            _users = Load();
        }

        public CertainDeathUser CreateGameUser(MyAppUser fbUser)
        {
            if (_users.Count(x => x.FBUser.Id.Equals(fbUser.Id)) == 0)
            {
                CertainDeathUser newUser = new CertainDeathUser() { FBUser = fbUser, WorldId = -1 };
                _users.Add(newUser);
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
            return (_users.Where(x => x.FBUser.Id.Equals(fbUser.Id)).FirstOrDefault());
        }

        private void Save()
        {
            try
            {
                Serialize(_users, "users.bin");
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
            System.IO.Stream ms = File.OpenWrite(String.Format("{0}\\{1}", _filePath, filename));
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
                FileStream fs = File.Open(String.Format("{0}\\{1}", _filePath, filename), FileMode.Open);

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



        public void GiveGameUserAGameWorldId(int userId, int worldId)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<CertainDeathUser> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MyAppUser> GetAllFbUsers()
        {
            throw new NotImplementedException();
        }


        public void UpdateGameUser(CertainDeathUser cdUser)
        {
            throw new NotImplementedException();
        }


        public CertainDeathUser GetGameUser(int userid)
        {
            throw new NotImplementedException();
        }
    }
}
