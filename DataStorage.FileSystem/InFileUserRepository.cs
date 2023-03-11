using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataStorage.Abstractions;
using DataStorage.Abstractions.Exceptions;
using Domain;
using StaticProxy.Abstractions;

namespace DataStorage.FileSystem
{
    public class InFileUserRepository : IUserRepository
    {
        private IFileOperations _fileOps;
        private string _path = new PathProvider().UserPath;
        private List<User> _users = new List<User>();
        private int _userCount;
        
        public InFileUserRepository(IFileOperations fileOps)
        {
            _fileOps = fileOps;
        }

        public void FillUserList()
        {
            string[] directories = _fileOps.GetDirectories(_path);
            foreach (string dir in directories)
            {
                FileInfo userInfo = new FileInfo(dir);
                var UserName = userInfo.Name;
                var UserPath = _path + "\\" + UserName;

                string[] files = _fileOps.GetDirectories(UserPath);
                User user = new User();

                foreach (string file in files)
                {
                    FileInfo accountInfo = new FileInfo(file);
                    var iban = accountInfo.Name;
                    var charArray = iban.ToCharArray();
                    var firstElement = charArray.ElementAt(0);
                    if (!Char.IsNumber(firstElement))
                    {
                        string[] content = new string[5];
                        var userFilePath = _path + "\\" + UserName + "\\" + UserName + ".txt";
                        content = _fileOps.ReadFromFile(userFilePath);

                        var userName = content[0];
                        var userPin = content[1];
                        var userId = content[2];

                        user.Name = userName;
                        user.Pin = userPin;
                        user.Id = Convert.ToInt32(userId);

                        _users.Add(user);
                        _userCount++;
                    }
                }
            }
        }

        public void Create(User user)
        {
            user.Id = ++_userCount;
            string[] content = new string[5];
            var userPath = _path + "\\" + user.Name + "\\" + user.Name + ".txt";
            content[0] = user.Name;
            content[1] = user.Pin;
            content[2] = user.Id.ToString();
            _fileOps.CreateDirectory(_path + "\\" + user.Name);
            _fileOps.WriteToFile(userPath, content);
        }

        public bool DoesUserExist(string username)
        {
            string[] files = _fileOps.GetDirectories(_path);
            foreach(string dir in files)
            {
                FileInfo info = new FileInfo(dir);
                var UserName = info.Name;
                if(UserName == username)
                {
                    return true;
                }
            }
            return false;
        }

        public User[] GetAllUsers()
        {
            string[] files = _fileOps.GetDirectories(_path);
            List<User> users = new List<User>();
            foreach (string dir in files)
            {
                FileInfo info = new FileInfo(dir);
                var UserName = info.Name;
                string[] content = new string[5];
                var userPath = _path + "\\" + UserName + "\\" + UserName + ".txt";

                content = _fileOps.ReadFromFile(userPath);
                var userName = content[0];
                var pin = content[1];
                var id = content[2];

                User user = new User(userName, pin);
                user.Id = Convert.ToInt16(id);
                users.Add(user);
                
            }
            return users.ToArray();
        }

        public User GetByUsername(string username)
        {
            try
            {
                string[] content = new string[5];
                var userPath = _path + "\\" + username + "\\" + username + ".txt";
                content = _fileOps.ReadFromFile(userPath);

                var userName = content[0];
                var pin = content[1];
                var id = content[2];
                User user = new User(userName, pin);
                user.Id = Convert.ToInt16(id);

                return user;
            }
            catch (Exception e)
            {
                throw new UserCouldNotBeFoundException($"Username {username} could not be found", e);
            }
        }
    }
}
