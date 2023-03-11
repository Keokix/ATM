using System;
using System.Collections.Generic;
using System.Linq;
using DataStorage.Abstractions;
using DataStorage.Abstractions.Exceptions;
using Domain;

namespace DataStorage.InMemory
{
    public class InMemoryUserRepository : IUserRepository
    {
        private int _userCount;
        private List<User> _userStorage = new List<User>();

        public void Create(User user)
        {
            user.Id = ++_userCount;
            _userStorage.Add(user);
        }

        public bool DoesUserExist(string username)
        {
            return _userStorage.Any(user => user.Name.Equals(username));
        }

        public User[] GetAllUsers()
        {
            return _userStorage.ToArray();
        }

        public User GetByUsername(string username)
        {
            try
            {
                return _userStorage.First(user => user.Name.Equals(username));
            }
            catch (InvalidOperationException e)
            {
                throw new UserCouldNotBeFoundException($"Username {username} could not be found", e);
            }
        }
    }
}
