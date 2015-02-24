using CertainDeathEngine.Models;
using CertainDeathEngine.Models.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CertainDeathEngine.DAL
{
    public class BasicUserDAL : IUserDAL
    {
        private string FilePath;
        private List<CertainDeathUser> users;
        private IGameDAL GameDAL;

        public BasicUserDAL(string path, IGameDAL gameDal)
        {
            FilePath = String.Format("{0}\\User", path);
            GameDAL = gameDal;
            users = Load();
        }

        public CertainDeathUser CreateGameUser(MyAppUser fbUser)
        {
            if (users.Where(x => x.FBUser.Email.Equals(fbUser.Email)).Count() == 0)
            {
                GameWorld newWorld = GameDAL.CreateWorld();
                CertainDeathUser newUser = new CertainDeathUser() { FBUser = fbUser, WorldId = newWorld.Id };
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
            //// Everyone can add their email here until we get a real user db to test with.
            //// We can each test with our own world id 

            //// Blake
            //if (fbUser.Id.Equals("812423582127592") && fbUser.Email.Equals("brollins90@gmail.com"))
            //{
            //    return new CertainDeathUser() { FBUser = fbUser, WorldId = 7 };
            //}
            //// Josh
            //else if (fbUser.Email.Equals("whoduexpect@gmail.com"))
            //{
            //    return new CertainDeathUser() { FBUser = fbUser, WorldId = 8 };
            //}
            //// Shayne
            //else if (fbUser.Email.Equals("shayne@email.com"))
            //{
            //    return new CertainDeathUser() { FBUser = fbUser, WorldId = 9 };
            //}
            //// Taylor
            //else if (fbUser.Email.Equals("taylor@email.com"))
            //{
            //    return new CertainDeathUser() { FBUser = fbUser, WorldId = 10 };
            //}
            //// Trevor
            //else if (fbUser.Email.Equals("trevor@email.com"))
            //{
            //    return new CertainDeathUser() { FBUser = fbUser, WorldId = 11 };
            //}
            ////return null;
            //return new CertainDeathUser() { WorldId = 7 };



        }

        private void Save()
        {
            try
            {
                Serialize(users, "users.bin");
            }
            catch (Exception)
            {
                // not much we can do....
                int i = 7;
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
                return new List<CertainDeathUser>();
            }
        }

    }
}
